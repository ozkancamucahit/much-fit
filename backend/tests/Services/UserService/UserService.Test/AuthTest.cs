using System.Net;
using System.Net.Http.Json;
using UserService.Api.Contracts;

namespace UserService.Test;

public sealed class AuthTest
  : IClassFixture<UserServiceTestFactory>
{
  private readonly HttpClient _client;

  public AuthTest(UserServiceTestFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task Register_With_Valid_Credentials_ShouldSucceed()
  {
    var email = $"user{Guid.CreateVersion7():N}@gmail.com";
    var result =
      await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "Password123!"));

    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    var body = await result.Content.ReadFromJsonAsync<AuthResponse>();
    Assert.NotNull(body);
    Assert.Equal(email, body.email);
    Assert.NotNull(body.token);
  }


  [Fact]
  public async Task Register_With_Duplicate_Email_Should_Fail()
  {
    var email = $"user{Guid.CreateVersion7():N}@gmail.com";
    var result1 = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "Password123!"));
    Assert.Equal(HttpStatusCode.OK, result1.StatusCode);
    var result2 =
      await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "Password123!"));

    Assert.Equal(HttpStatusCode.Conflict, result2.StatusCode);
  }

  [Fact]
  public async Task Login_With_Valid_Credentials_ShouldSucceed()
  {
    var email = $"user{Guid.CreateVersion7():N}@gmail.com";
    var payload = new RegisterRequest(email, "Password123!");
    await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "Password123!"));
    var result =
      await _client.PostAsJsonAsync("/api/auth/login", payload);

    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    var body = await result.Content.ReadFromJsonAsync<AuthResponse>();
    Assert.NotNull(body);
    Assert.Equal(email, body.email);
    Assert.NotNull(body.token);
  }

  [Fact]
  public async Task Login_With_InValid_Credentials_Password_ShouldFail()
  {
    var email = $"user{Guid.CreateVersion7():N}@gmail.com";
    var payload = new RegisterRequest(email, "Password123!465");
    var result =
      await _client.PostAsJsonAsync("/api/auth/login", payload);

    Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
  }
  
  [Fact]
  public async Task Login_With_InValid_Credentials_Email_ShouldFail()
  {
    var email = "wrong@gmail.com";
    var payload = new RegisterRequest(email, "Password123!");
    var result =
      await _client.PostAsJsonAsync("/api/auth/login", payload);

    Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
  }


}
