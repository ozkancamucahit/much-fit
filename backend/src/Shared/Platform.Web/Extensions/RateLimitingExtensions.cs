using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Web.Extensions;

public static class RateLimitingExtensions
{
  public const string AuthPolicy = "auth";

  public static IServiceCollection AddPlatformRateLimiting(this IServiceCollection services)
  {
    services
      .AddRateLimiter(options =>
      {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>

          RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            { PermitLimit = 300, Window = TimeSpan.FromMinutes(10), QueueLimit = 0 }
          ));

        options.AddPolicy(AuthPolicy, context =>
          RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            { PermitLimit = 10, Window = TimeSpan.FromMinutes(10), QueueLimit = 0 }));
      });

      return services;
  }
}
