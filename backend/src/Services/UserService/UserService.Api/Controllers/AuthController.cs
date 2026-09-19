using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
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
  private readonly IStringLocalizer<SharedResource> _localizer;

  public AuthController(
    UserManager<ApplicationUser> userManager,
    JWTTokenService tokenService,
    IStringLocalizer<SharedResource> localizer)
  {
    _userManager = userManager;
    _tokenService = tokenService;
    _localizer = localizer;
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

    var userExists = (await _userManager.FindByEmailAsync(normalizedEmail)) is not null;
    if(userExists)
    {
        return Conflict(_localizer["EmailAlreadyRegistered"].Value);
    }

    var result = await _userManager.CreateAsync(user, request.Password.Trim());

    if (!result.Succeeded)
    {
      if (result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
      {
        return Conflict(_localizer["EmailAlreadyRegistered"].Value);
      }
      return BadRequest(result.Errors);
    }

    var token = _tokenService.GenerateToken(user);
    return Ok(new AuthResponse(user.Id, user.Email, token));
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponse>> Login(
    LoginRequest request,
    CancellationToken cancellationToken)
  {
    var normalizedEmail = request.email.Trim().ToLowerInvariant();
    var user = await _userManager.FindByEmailAsync(normalizedEmail);

    if (user is null)
    {
      return Unauthorized(_localizer["InvalidEmailOrPassword"].Value);
    }

    bool passwordValid = await _userManager.CheckPasswordAsync(user, request.password);

    if (!passwordValid)
    {
      return Unauthorized(_localizer["InvalidEmailOrPassword"].Value);
    }

    var token = _tokenService.GenerateToken(user);
    return Ok(new AuthResponse(user.Id, user.Email!, token));
  }



}
