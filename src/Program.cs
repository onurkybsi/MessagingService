using System;
using System.IO;
using MessagingService.Infrastructure;
using MessagingService.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MessagingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = InitialHelper.GetConfiguration(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            Log.Logger = InitialHelper.CreateELKLogger(new ELKLoggerConfig
            {
                AppName = configuration["ASPNETCORE_APPLICATIONNAME"],
                ElasticsearchURL = configuration["ElasticsearchUrl"]
            });

            var host = CreateHostBuilder(args, configuration).Build();

            CreateAdmin(host.Services.GetService(typeof(IUserService)) as IUserService, configuration);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
            => Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(config => config.ClearProviders()).UseSerilog();

        private static void CreateAdmin(IUserService userService, IConfiguration configuration)
        {
            userService.CreateUser(new Model.User
            {
                Username = configuration["AdminInfo:Username"],
                HashedPassword = Service.EncryptionHelper.CreateHashed(configuration["AdminInfo:Password"]),
                Role = Model.Constants.MessageHub.Role.Admin,
            });
        }
    }
}
