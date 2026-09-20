using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ApiGateway.Api;

public sealed class SecurityHeadersMiddleware
{
  private readonly RequestDelegate _next;
  private readonly Dictionary<string, string> _headers;

  public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
  {
    _next = next;
    _headers = configuration
      .GetSection("SecurityHeaders")
      .GetChildren()
      .Where(child => !string.IsNullOrWhiteSpace(child.Value))
      .ToDictionary(child => child.Key, child => child.Value!, StringComparer.OrdinalIgnoreCase);
  }

  public async Task InvokeAsync(HttpContext context)
  {
    context.Response.OnStarting(() =>
    {
      foreach (var (name, value) in _headers)
      {
        context.Response.Headers[name] = value;
      }

      return Task.CompletedTask;
    });

    await _next(context);
  }
}

public static class SecurityHeadersExtensions
{
  public static WebApplication UseSecurityHeaders(this WebApplication app)
  {
    app.UseMiddleware<SecurityHeadersMiddleware>();

    return app;
  }
}