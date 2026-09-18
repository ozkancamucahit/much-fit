using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Api.Entities;

namespace UserService.Api.Data;

public sealed class UserDbContext
  : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
  #region CTOR
  public UserDbContext(DbContextOptions<UserDbContext> options)
    : base(options)
  {

  }
  #endregion

  #region DBSETS
  public DbSet<UserProfile> UserProfileCollection => Set<UserProfile>();
  public DbSet<ApplicationUser> ApplicationUserCollection => Set<ApplicationUser>();

  #endregion


  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    UserProfileBuilder.Configure(builder.Entity<UserProfile>());
  }

}
