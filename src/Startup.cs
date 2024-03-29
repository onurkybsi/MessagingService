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
using Microsoft.EntityFrameworkCore;
using EnvironmentName = Microsoft.Extensions.Hosting.EnvironmentName;

namespace MessagingService {

  public class Startup {

    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services) {
      ConfigureAuth(services);
      ConfigureRepositories(services);
      ConfigureBusinessServices(services);

      services.AddDbContext<MessagingServiceDbContext>(options =>
          options.UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw"));

      services.AddCors(o => o.AddPolicy("MessagingServicePolicy", builder => {
        string[] allowedOrigins = Configuration.GetStringArray("AllowedOrigin");
        builder.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
      }));

      services.AddControllers().AddNewtonsoftJson();
      services.AddSignalR();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
      else if (env.IsProduction())
        app.UseHttpsRedirection();

      app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = (diagnosticContext, httpContext) => {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
      });

      app.UseStaticFiles(new StaticFileOptions {
        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "TestClient")),
        RequestPath = "/test"
      });

      app.UseRouting();
      app.UseCors("MessagingServicePolicy");

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
        endpoints.MapHub<MessageHub>("/messagehub");
      });

      Log.ForContext<Startup>().Information("{Application} is listening on {Env}...", env.ApplicationName, env.EnvironmentName);
    }

    private void ConfigureAuth(IServiceCollection services) {
      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(options => {
        options.RequireHttpsMetadata = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateAudience = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development,
          ValidateIssuer = Configuration.GetAspNetCoreEnvironmentName() != EnvironmentName.Development,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = Configuration["JwtAuthConfiguration.Issuer"],
          ValidAudience = Configuration["JwtAuthConfiguration.Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtAuthConfiguration.SecurityKey"])),
          ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents {
          OnMessageReceived = context => {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/messagehub"))) {
              context.Token = accessToken;
            }
            return Task.CompletedTask;
          },

          OnAuthenticationFailed = context => {
            Log.Error($"Authentication was refused !: {context.Exception.Message}");
            return Task.CompletedTask;
          },
        };
      });
    }

    private void ConfigureRepositories(IServiceCollection services) {
      MongoDBSettings messagingServiceDbSettings = new MongoDBSettings {
        ConnectionString = Configuration["MessagingServiceDb_ConnectionString"],
        DatabaseName = Configuration["MessagingServiceDb_DatabaseName"]
      };

      services.AddSingleton<IUserRepository>(ur => new UserRepository(new MongoDBCollectionSettings {
        DatabaseSettings = messagingServiceDbSettings,
        CollectionName = "user",
        CreateCollectionOptions = new MongoDB.Driver.CreateCollectionOptions {
          Collation = new MongoDB.Driver.Collation("tr", true)
        }
      }));
      services.AddSingleton<IMessageRepository>(mr => new MessageRepository(new MongoDBCollectionSettings { DatabaseSettings = messagingServiceDbSettings, CollectionName = "message" }));
      services.AddSingleton<IMessageGroupRepository>(mg => new MessageGroupRepository(new MongoDBCollectionSettings { DatabaseSettings = messagingServiceDbSettings, CollectionName = "messagegroup" }));
    }

    private void ConfigureBusinessServices(IServiceCollection services) {
      services.AddSingleton<IUserService, UserService>();
      services.AddSingleton<IMessageService, MessageService>();
      services.AddSingleton<IAuthService>(auth => new JwtAuthService(new JwtAuthSettings {
        Issuer = Configuration["JwtAuthSettings_Issuer"],
        Audience = Configuration["JwtAuthSettings_Audience"],
        SecurityKey = Configuration["JwtAuthSettings_SecurityKey"]
      }, auth.GetRequiredService<IUserService>()));
      services.AddSingleton<IMessageHubService, MessageHubService>();
    }

  }

}
