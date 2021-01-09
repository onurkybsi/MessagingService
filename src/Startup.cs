using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Data;
using MessagingService.Hubs;
using MessagingService.Infrastructure;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MessagingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuth(services);
            ConfigureRepositories(services);
            ConfigureBusinessServices(services);

            services.AddCors(o => o.AddPolicy("MessagingServicePolicy", builder =>
            {
                string[] allowedOrigins = Configuration.GetSection("ALLOWED_ORIGINS").Get<string[]>();

                builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddControllers().AddNewtonsoftJson();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else if (env.IsProduction())
                app.UseHttpsRedirection();

            // testClient.html i ekler.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "TestClient")),
                RequestPath = "/test"
            });

            app.UseRouting();
            app.UseCors("MessagingServicePolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/messagehub");
            });
        }

        public void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWTSettings:Issuer"],
                        ValidAudience = Configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SecurityKey"])),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/messagehub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.Configure<JwtAuthSettings>(options => Configuration.GetSection("JwtAuthSettings").Bind(options));
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            MongoDBSettings messagingServiceDbSettings = new MongoDBSettings
            {
                ConnectionString = Configuration["MessagingServiceDb:ConnectionString"],
                DatabaseName = Configuration["MessagingServiceDb:DatabaseName"],
            };

            services.AddSingleton<IUserRepository>(ur => new UserRepository(new MongoDBCollectionSettings { DatabaseSettings = messagingServiceDbSettings, CollectionName = "user" }));
            services.AddSingleton<IMessageRepository>(mr => new MessageRepository(new MongoDBCollectionSettings { DatabaseSettings = messagingServiceDbSettings, CollectionName = "messsage" }));
        }

        private void ConfigureBusinessServices(IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IAuthService, JwtAuthService>();
        }
    }
}
