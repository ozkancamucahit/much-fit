using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Consul;

namespace Central.Common.Lib.CustomExtensions.Consul;

public static class ConsulExtensions
{
  public static IServiceCollection SetupConsul(
    this IServiceCollection services
    // ,IConfiguration configuration
    )
  {
    // var options = configuration.GetOptions<ConsulOptions>(ConsulSectionName);
    services
      .AddConsulDiscoveryClient();
    return services;
  }
}
