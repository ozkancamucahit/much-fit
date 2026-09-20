using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
        .WriteTo.Console(
          theme: AnsiConsoleTheme.Code,
          outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({Service}) {CorrelationId} {Message:lj}{NewLine}{Exception}"
        )
      );
    return builder;
  }

}
