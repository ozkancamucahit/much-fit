
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Entities;

namespace UserService.Api.Auth;

public sealed class JWTTokenService
{

  private readonly IConfiguration _configuration;

  public JWTTokenService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public string GenerateToken(ApplicationUser user)
  {

    ArgumentNullException.ThrowIfNull(user);

    var privateKeyPem = _configuration.GetValue<string>("Jwt:PrivateKeyPem");

    ArgumentException.ThrowIfNullOrWhiteSpace(privateKeyPem);

    var issuer = _configuration.GetValue<string>("Jwt:Issuer") ?? "UserService";
    var audience = _configuration.GetValue<string>("Jwt:Audience") ?? "MuchFit";
    var expiryMinutes = int.Parse(_configuration.GetValue<string>("Jwt:ExpiryMinutes") ?? "120", CultureInfo.InvariantCulture);

    var claims = new[]
    {
      new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,
      user.Id.ToString()),
      new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email,
      user.Email!),
      new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,
      Guid.NewGuid().ToString()),
    };

    using var rsa = RSA.Create();
    rsa.ImportFromPem(privateKeyPem);

    var key = new RsaSecurityKey(rsa.ExportParameters(true));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

    var token = new JwtSecurityToken(
      issuer: issuer,
      audience: audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
