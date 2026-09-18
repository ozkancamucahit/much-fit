using Microsoft.AspNetCore.Identity;

namespace UserService.Api.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
  public UserProfile? UserProfile { get; set; }
}
