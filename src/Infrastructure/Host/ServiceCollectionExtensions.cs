using Microsoft.Extensions.Configuration;

namespace MessagingService.Infrastructure {
  public static class ServiceCollectionExtensions {
    public static string GetAspNetCoreEnvironmentName(this IConfiguration configuration)
        => configuration["ASPNETCORE_ENVIRONMENT"];

    public static string[] GetStringArray(this IConfiguration configuration, string arrayKey)
        => configuration.GetSection(arrayKey).Get<string[]>();
  }
}