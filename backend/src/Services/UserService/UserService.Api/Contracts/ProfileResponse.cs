namespace UserService.Api.Contracts;

public record ProfileResponse(
  Guid userId,
  int age,
  string sex,
  double heightCm,
  double weightKg,
  string activityLEvel,
  string goal,
  DateTimeOffset updatedAt
);
