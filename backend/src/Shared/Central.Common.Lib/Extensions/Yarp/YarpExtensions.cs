using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Central.Common.Lib.Extensions.Yarp;

public static class YarpExtensions
{
  public static IReverseProxyBuilder AddYarp(
    this IServiceCollection services
    , IConfiguration configuration
    , string configSection
    )
  {
    return services
      .AddReverseProxy()
      .LoadFromConfig(configuration.GetSection(configSection));
  }

  public static WebApplication MapYarp(this WebApplication app)
  {
    app.MapReverseProxy();

    return app;
  }
}
