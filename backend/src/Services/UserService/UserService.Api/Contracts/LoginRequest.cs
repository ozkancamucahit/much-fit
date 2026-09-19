using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Contracts;

public record LoginRequest(
  [Required, EmailAddress] string email,
  [Required, DataType(DataType.Password)] string password
);
