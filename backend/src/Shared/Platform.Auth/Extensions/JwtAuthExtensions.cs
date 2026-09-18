using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Platform.Auth.Extensions;

public static class JwtAuthExtensions
{
  public static IServiceCollection AddPlatformJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment environment)
  {
    var publicKeyPem = configuration.GetValue<string>("Jwt:PublicKeyPem");

    ArgumentNullException.ThrowIfNull(environment);

    if (string.IsNullOrWhiteSpace(publicKeyPem))
    {
      var publicKeyPath = Path.Combine(environment.ContentRootPath,
        ".."
        , ".."
        , ".."
        , "Shared"
        , "Keys"
        , "jwt-public.pem"
      );
      publicKeyPem = File.ReadAllText(publicKeyPath);
    }

    var rsa = RSA.Create();
    rsa.ImportFromPem(publicKeyPem);
    var signinKey = new RsaSecurityKey(rsa);

    services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = configuration.GetValue<string>("Jwt:Issuer") ?? "UserService",
          ValidAudience = configuration.GetValue<string>("Jwt:Audience") ?? "MuchFit",
          IssuerSigningKey = signinKey
        };
      });

    services
      .AddAuthorization();

    return services;
  }


  public static Guid GetUserId(this ClaimsPrincipal principal)
  {
    var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    ArgumentException.ThrowIfNullOrWhiteSpace(sub);
    return Guid.Parse(sub);
  }


}
