using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Platform.Web.Extensions;
using UserService.Api.Auth;
using UserService.Api.Contracts;
using UserService.Api.Entities;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
public class AuthController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly JWTTokenService _tokenService;

  public AuthController(
    UserManager<ApplicationUser> userManager,
    JWTTokenService tokenService)
  {
    _userManager = userManager;
    _tokenService = tokenService;
  }


  [HttpPost("register")]
  public async Task<ActionResult<AuthResponse>> Register(
    RegisterRequest request,
    CancellationToken cancellationToken)
  {
    var normalizedEmail = request.Email.Trim().ToLowerInvariant();

    var user = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      UserName = normalizedEmail,
      Email = normalizedEmail,
    };

    var result = await _userManager.CreateAsync(user, request.Password.Trim());

    if (!result.Succeeded)
    {
      if (result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
      {
        return Conflict("An account with that email is already registered");
      }
      return BadRequest(result.Errors);
    }

    var token = _tokenService.GenerateToken(user);
    return Ok(new AuthResponse(user.Id, user.Email, token));
  }



}
