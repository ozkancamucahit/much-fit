using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Platform.Auth;

public static class TestJwtTokenFactory
{
  public static string CreateToken(
    RSA signingKey,
    Guid userId,
    DateTime? expires = null)
  {
    var claims = new[]
    {
      new Claim(
        Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,
        userId.ToString()),
      new Claim(
        Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email,
        $"test@{signingKey}.com"),
      new Claim(
        Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,
        Guid.NewGuid().ToString()),
    };

    var key = new RsaSecurityKey(signingKey.ExportParameters(true));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

    var token = new JwtSecurityToken(
      issuer: "UserService",
      audience: "MuchFit",
      claims: claims,
      expires: expires ?? DateTime.UtcNow.AddHours(1),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
    
  }

}
