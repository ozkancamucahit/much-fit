using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Contracts;

public record RegisterRequest(
  [Required(ErrorMessage = "Validation_Required"),
   EmailAddress(ErrorMessage = "Validation_InvalidEmail"),
   Display(Name = "Field_Email")] string Email,
  [Required(ErrorMessage = "Validation_Required"),
   DataType(DataType.Password),
   Display(Name = "Field_Password")] string Password
);
