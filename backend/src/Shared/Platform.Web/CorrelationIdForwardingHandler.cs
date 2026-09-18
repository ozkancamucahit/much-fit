using Central.Common.Lib.Extensions.Observability;
using Microsoft.AspNetCore.Http;

namespace Platform.Web;

public sealed class CorrelationIdForwardingHandler
  : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CorrelationIdForwardingHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();
    if (!string.IsNullOrWhiteSpace(correlationId))
    {
      request.Headers.AddCorrelationId(correlationId);
    }

    return base.SendAsync(request, cancellationToken);
  }

}
