using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Contracts;

public record RegisterRequest(
  [Required, EmailAddress]string Email,
  [Required, DataType(DataType.Password)]string Password
);
