using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Action;
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
using Serilog;
using EnvironmentName = Microsoft.Extensions.Hosting.EnvironmentName;

namespace MessagingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static IServiceProvider ServiceProvider { get; set; }

        public static T GetInstance<T>()
            => ServiceProvider.GetRequiredService<T>();

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuth(services);
            ConfigureRepositories(services);
            ConfigureBusinessServices(services);

            services.AddCors(o => o.AddPolicy("MessagingServicePolicy", builder =>
            {
                string[] allowedOrigins = Configuration.GetSection("AllowedOrigin").Get<string[]>();

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

            app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            });

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

            ServiceProvider = app.ApplicationServices;
            Log.ForContext<Startup>().Information("{Application} is listening on {Env}...", env.ApplicationName, env.EnvironmentName);
        }

        public void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development,
                        ValidateIssuer = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtAuthSettings_Issuer"],
                        ValidAudience = Configuration["JwtAuthSettings_Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtAuthSettings_SecurityKey"])),
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
                        },

                        OnAuthenticationFailed = context =>
                        {
                            Log.Error($"Authentication was refused !: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                    };
                });
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            MongoDBSettings messagingServiceDbSettings = new MongoDBSettings
            {
                ConnectionString = Configuration["MessagingServiceDb_ConnectionString"],
                DatabaseName = Configuration["MessagingServiceDb_DatabaseName"]
            };

            services.AddSingleton<IUserRepository>(ur => new UserRepository(new MongoDBCollectionSettings
            {
                DatabaseSettings = messagingServiceDbSettings,
                CollectionName = "user",
                CreateCollectionOptions = new MongoDB.Driver.CreateCollectionOptions
                {
                    Collation = new MongoDB.Driver.Collation("tr", true)
                }
            }));
            services.AddSingleton<IMessageRepository>(mr => new MessageRepository(new MongoDBCollectionSettings { DatabaseSettings = messagingServiceDbSettings, CollectionName = "message" }));
        }

        private void ConfigureBusinessServices(IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IAuthService>(auth => new JwtAuthService(new JwtAuthSettings
            {
                Issuer = Configuration["JwtAuthSettings_Issuer"],
                Audience = Configuration["JwtAuthSettings_Audience"],
                SecurityKey = Configuration["JwtAuthSettings_SecurityKey"]
            }, auth.GetRequiredService<IUserService>()));
            services.AddSingleton<IMessageHubService, MessageHubService>();
        }

        private void SetupActions(IServiceCollection services)
        {
            services.AddSingleton<IBlockUserAction>(bua => new BlockUserAction(new List<IBlockUser>
            {
                bua.GetRequiredService<MessageHubService>(),
            }, new List<IBlockUserAsync>
            {
                bua.GetRequiredService<UserService>()
            }));

            services.AddSingleton<ICreateMessageGroupAction>(bua => new CreateMessageGroupAction(new List<ICreateMessageGroup>
            {
                bua.GetRequiredService<MessageHubService>(),
            }, new List<ICreateMessageGroupAsync>
            {
                bua.GetRequiredService<MessageService>()
            }));
        }
    }
}
