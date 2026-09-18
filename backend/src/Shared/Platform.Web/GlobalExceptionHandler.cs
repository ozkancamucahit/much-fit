using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Platform.Web;

public sealed class GlobalExceptionHandler
  : IExceptionHandler
{

  private readonly ILogger<GlobalExceptionHandler> _logger;

  public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
  {

    _logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
      httpContext.Request.Method, httpContext.Request.Path);

    var problemDetails = new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "An unexpected error occured",
      Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };

    httpContext.Response.StatusCode = problemDetails.Status.Value;
    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}

public static class GlobalExceptionHandlerExtensions
{
  public static IServiceCollection AddPlatformExceptionHandling(this IServiceCollection services)
  {
    services
      .AddExceptionHandler<GlobalExceptionHandler>()
      .AddProblemDetails();
    return services;
  }
}




