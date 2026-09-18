using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Platform.Web.Extensions;

public static class LoggingExtensions
{

  public static WebApplicationBuilder AddLogging(
    this WebApplicationBuilder builder,
    string serviceName
  )
  {
    builder
      .Host
      .UseSerilog((context, services, configuration) =>
        configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", serviceName)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} ({Service}) {CorrelationId} {Message:lj}{NewLine}{Exception} ]")
      );
    return builder;
  }

}
