using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace MessagingService.Infrastructure
{
    public static class InitialHelper
    {
        public static Serilog.ILogger CreateELKLogger(ELKLoggerConfig config)
        {
            if (config is null || string.IsNullOrEmpty(config.AppName) || string.IsNullOrEmpty(config.ElasticsearchURL))
                throw new ArgumentNullException(nameof(ELKLoggerConfig) + "must be passed");

            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", config.AppName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri(config.ElasticsearchURL))
                    {
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = string.Format("{0}-logs", config.AppName.ToLower())
                    })
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

        public static IConfiguration GetConfiguration(string basePath, string environment)
            => new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
    }
}