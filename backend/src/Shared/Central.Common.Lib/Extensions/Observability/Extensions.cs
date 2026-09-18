using System.Net.Http.Headers;
using Central.Common.Lib.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Context;

namespace Central.Common.Lib.Extensions.Observability;

public static class Extensions
{
  public const string CorrelationIdKey = "x-correlation-id";

  public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
      => app.Use(async (ctx, next) =>
      {
        if (!ctx.Request.Headers.TryGetValue(CorrelationIdKey, out var correlationId))
        {
          correlationId = Guid.CreateVersion7().ToString("N");
        }

        ctx.Items[CorrelationIdKey] = correlationId.ToString();
        ctx.Response.Headers[CorrelationIdKey] = correlationId;
        ctx.Request.Headers[CorrelationIdKey] = correlationId;
        using (LogContext.PushProperty("CorrelationId", correlationId))
          await next();
      });

  public static string? GetCorrelationId(this HttpContext context)
      => context.Items.TryGetValue(CorrelationIdKey, out var correlationId) ? correlationId as string : null;

  public static void AddCorrelationId(this HttpRequestHeaders headers, string correlationId)
      => headers.TryAddWithoutValidation(CorrelationIdKey, correlationId);

  public static IServiceCollection SetupEnvironmentConfig(
    this IServiceCollection services
    , string envName
    )
  {
    if (String.IsNullOrWhiteSpace(envName))
    {
      throw new ArgumentNullException("envName", "Environment name is required");
    }

    services
      .AddSingleton(sp => new EnvironmentConfig
      {
        IsDevelopmentEnvironment = envName == Environments.Development,
        IsProductionEnvironment = envName == Environments.Production,
        IsStagingEnvironment = envName == Environments.Staging || envName == "Sandbox"
      });
    return services;
  }
}
