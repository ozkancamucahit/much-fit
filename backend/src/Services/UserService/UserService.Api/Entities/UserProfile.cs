using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService.Api.Entities;

public sealed class UserProfile
{
  public Guid UserId { get; set; }
  public ApplicationUser? User { get; set; }
  public int Age { get; set; }
  public string Sex { get; set; } = string.Empty;
  public double HeightCm { get; set; }
  public double WeightKg { get; set; }
  public ActivityLevel ActivityLevel { get; set; }
  public FitnessGoal FitnessGoal { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
}


public enum ActivityLevel
{
  Sedentary = 0,
  LightlyActive = 1,
  ModeratelyActive = 2,
  VeryActive = 3,
  ExtraActive = 4,
}

public enum FitnessGoal
{
  Cut = 0,
  Maintain = 1,
  Bulk = 2,
}


public static class UserProfileBuilder
{
  public static void Configure(EntityTypeBuilder<UserProfile> builder)
  {

    builder
      .ToTable("user_profile");

    builder
      .HasKey(e => e.UserId)
      .HasName("pk_user_profile");
  }
}

