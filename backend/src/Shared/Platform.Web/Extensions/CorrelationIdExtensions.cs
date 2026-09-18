using Microsoft.Extensions.DependencyInjection;

namespace Platform.Web.Extensions;

public static class CorrelationIdExtensions
{
  public static IServiceCollection AddCorrelationId(this IServiceCollection services)
  {
    services
      .AddHttpContextAccessor()
      .AddTransient<CorrelationIdForwardingHandler>();
    return services;
  }
}
