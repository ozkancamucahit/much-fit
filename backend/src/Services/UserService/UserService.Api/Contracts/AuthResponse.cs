namespace UserService.Api.Contracts;

public record AuthResponse(
  Guid userId,
  string email,
  string token
);
