using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Platform.Web.Extensions;

public static class LoggingExtensions
{

  private static readonly ConsoleTheme ColorTheme = new AnsiConsoleTheme(
    new Dictionary<ConsoleThemeStyle, string>
    {
      [ConsoleThemeStyle.Text] = "\x1b[0m",
      [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0008m",
      [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0008m",
      [ConsoleThemeStyle.Invalid] = "\x1b[0m",
      [ConsoleThemeStyle.Null] = "\x1b[0m",
      [ConsoleThemeStyle.Name] = "\x1b[0m",
      [ConsoleThemeStyle.String] = "\x1b[0m",
      [ConsoleThemeStyle.Number] = "\x1b[0m",
      [ConsoleThemeStyle.Boolean] = "\x1b[0m",
      [ConsoleThemeStyle.Scalar] = "\x1b[0m",
      [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;0008m",
      [ConsoleThemeStyle.LevelDebug] = "\x1b[38;5;0008m",
      [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;0010m",
      [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;0011m",
      [ConsoleThemeStyle.LevelError] = "\x1b[38;5;0009m",
      [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;0015m\x1b[48;5;0196m",
    }
  );

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
          theme: ColorTheme,
          applyThemeToRedirectedOutput: true,
          outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({Service}) {CorrelationId} {Message:lj}{NewLine}{Exception}"
        )
      );
    return builder;
  }

}
