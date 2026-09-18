using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Central.Common.Lib.Extensions.Serilog;

public static class SerilogExtensions
{
  public static WebApplicationBuilder AddLoggerConfigs(
    this WebApplicationBuilder builder,
    string? filePath = null)
  {
    builder.Host.UseSerilog((ctx, services, lc) =>
    {
      lc.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
        .WriteTo.Console(
          theme: AnsiConsoleTheme.Code,
          applyThemeToRedirectedOutput: true
        );

      if (!string.IsNullOrWhiteSpace(filePath))
      {
        lc.WriteTo.File(filePath);
      }
    }, writeToProviders: true);

    return builder;
  }
}

