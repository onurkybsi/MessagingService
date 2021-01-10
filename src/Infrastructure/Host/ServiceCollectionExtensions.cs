using Microsoft.Extensions.Configuration;

namespace MessagingService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static string GetAspNetCoreEnvironmentName(this IConfiguration configuration)
            => configuration["ASPNETCORE_ENVIRONMENT"];
    }
}