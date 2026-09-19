using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Contracts;

public record ProfileRequest(
  [Required, Range(minimum: 14, maximum: 120)] int age,
  [Required] string sex,
  [Range(50, 300)] double heightCm,
  [Range(40, 250)] double weightKg,
  [Required] string activityLevel,
  [Required] string Goal
);
