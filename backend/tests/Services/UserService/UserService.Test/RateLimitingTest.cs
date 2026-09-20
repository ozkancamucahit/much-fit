using System.Net;
using System.Net.Http.Json;
using UserService.Api.Contracts;

namespace UserService.Test;

public sealed class RateLimitingTest
  : IClassFixture<UserServiceTestFactory>
{
  private readonly HttpClient _client;

  public RateLimitingTest(UserServiceTestFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task RateLimiting_WithTooManyRequests_Get429()
  {
    HttpResponseMessage? lastResponse = null;
    var payload = new RegisterRequest("rate@limit.com", "Password123!");
    var registerResponse= await _client.PostAsJsonAsync("/api/auth/register", payload);
    Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

    for (int i = 0; i < 20; i++)
    {
      lastResponse= await _client.PostAsJsonAsync("/api/auth/login", payload);
      if (lastResponse.StatusCode == HttpStatusCode.TooManyRequests)
      {
        break;
      }
    }
      Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse?.StatusCode);
  }
}
