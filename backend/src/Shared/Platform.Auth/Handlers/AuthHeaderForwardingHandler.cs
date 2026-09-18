using Microsoft.AspNetCore.Http;

namespace Platform.Auth.Handlers;

public sealed class AuthHeaderForwardingHandler
  : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuthHeaderForwardingHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {

    var incomingAuthHeader = _httpContextAccessor.HttpContext?
      .Request.Headers
      .Authorization
      .ToString();

    if (!string.IsNullOrEmpty(incomingAuthHeader))
    {
      request.Headers.TryAddWithoutValidation("Authorization", incomingAuthHeader);
    }

    return base.SendAsync(request, cancellationToken);
  }
}
