using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UserService.Api.Contracts;
using UserService.Api.Data;
using UserService.Api.Entities;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProfileController
  (UserDbContext db,
  IStringLocalizer<SharedResource> localizer)
  : ControllerBase
{

  [HttpGet]
  public async Task<ActionResult<ProfileResponse>> GetProfile(CancellationToken cancellationToken)
  {
    string? input = User.FindFirstValue(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub);
    ArgumentException.ThrowIfNullOrWhiteSpace(input);

    var currentUserId = Guid.Parse(input);
    var profile = await db
      .UserProfileCollection
      .AsNoTracking()
      .TagWith("GetProfile_linq")
      .FirstOrDefaultAsync(p => p.UserId == currentUserId, cancellationToken)
      .ConfigureAwait(false);

    if (profile is null)
    {
      return NotFound("Profile has not been completed yet");
    }

    _ = localizer;

    return Ok(new ProfileResponse(
      profile.UserId,
      profile.Age,
      profile.Sex,
      profile.HeightCm,
      profile.WeightKg,
      profile.ActivityLevel.ToString(),
      profile.FitnessGoal.ToString(),
      profile.UpdatedAt));
  }

  [HttpPut]
  public async Task<ActionResult<ProfileResponse>> UpsertProfile(
    [FromBody] ProfileRequest request,
    CancellationToken cancellationToken)
  {

    ArgumentNullException.ThrowIfNull(request);

    if (!Enum.TryParse<ActivityLevel>(request.activityLevel, ignoreCase: true, out var activityLevel))
    {
      return BadRequest("Activity level is invalid");
    }

    if (!Enum.TryParse<FitnessGoal>(request.Goal, ignoreCase: true, out var goal))
    {
      return BadRequest("fitness goal is invalid");
    }

    string? input = User.FindFirstValue(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub);
    ArgumentException.ThrowIfNullOrWhiteSpace(input);

    var currentUserId = Guid.Parse(input);
    var profile = await db
      .UserProfileCollection
      .TagWith("UpsertProfile_GetProfile_linq")
      .FirstOrDefaultAsync(p => p.UserId == currentUserId, cancellationToken)
      .ConfigureAwait(false);

    if (profile is null)
    {
      return Forbid("Unautorized request");
    }

    profile.Age = request.age;
    profile.Sex = request.sex;
    profile.HeightCm = request.heightCm;
    profile.WeightKg = request.weightKg;
    profile.ActivityLevel = activityLevel;
    profile.FitnessGoal = goal;
    profile.UpdatedAt = DateTimeOffset.UtcNow;

    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    return Accepted(new ProfileResponse(
      profile.UserId,
      profile.Age,
      profile.Sex,
      profile.HeightCm,
      profile.WeightKg,
      profile.ActivityLevel.ToString(),
      profile.FitnessGoal.ToString(),
      profile.UpdatedAt));
  }


}
