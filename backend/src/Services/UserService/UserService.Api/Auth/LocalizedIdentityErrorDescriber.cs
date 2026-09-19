using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace UserService.Api.Auth;

public sealed class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
  private readonly IStringLocalizer<SharedResource> _localizer;

  public LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResource> localizer)
  {
    _localizer = localizer;
  }

  public override IdentityError DefaultError() =>
    NewIdentityError(nameof(DefaultError), _localizer[nameof(DefaultError)].Value);

  public override IdentityError ConcurrencyFailure() =>
    NewIdentityError(nameof(ConcurrencyFailure), _localizer[nameof(ConcurrencyFailure)].Value);

  public override IdentityError DuplicateEmail(string email) =>
    NewIdentityError(nameof(DuplicateEmail), _localizer[nameof(DuplicateEmail), email].Value);

  public override IdentityError DuplicateUserName(string userName) =>
    NewIdentityError(nameof(DuplicateUserName), _localizer[nameof(DuplicateUserName), userName].Value);

  public override IdentityError InvalidEmail(string? email) =>
    NewIdentityError(nameof(InvalidEmail), _localizer[nameof(InvalidEmail), email ?? string.Empty].Value);

  public override IdentityError InvalidUserName(string? userName) =>
    NewIdentityError(nameof(InvalidUserName), _localizer[nameof(InvalidUserName), userName ?? string.Empty].Value);

  public override IdentityError InvalidToken() =>
    NewIdentityError(nameof(InvalidToken), _localizer[nameof(InvalidToken)].Value);

  public override IdentityError LoginAlreadyAssociated() =>
    NewIdentityError(nameof(LoginAlreadyAssociated), _localizer[nameof(LoginAlreadyAssociated)].Value);

  public override IdentityError PasswordMismatch() =>
    NewIdentityError(nameof(PasswordMismatch), _localizer[nameof(PasswordMismatch)].Value);

  public override IdentityError PasswordRequiresDigit() =>
    NewIdentityError(nameof(PasswordRequiresDigit), _localizer[nameof(PasswordRequiresDigit)].Value);

  public override IdentityError PasswordRequiresLower() =>
    NewIdentityError(nameof(PasswordRequiresLower), _localizer[nameof(PasswordRequiresLower)].Value);

  public override IdentityError PasswordRequiresNonAlphanumeric() =>
    NewIdentityError(nameof(PasswordRequiresNonAlphanumeric), _localizer[nameof(PasswordRequiresNonAlphanumeric)].Value);

  public override IdentityError PasswordRequiresUpper() =>
    NewIdentityError(nameof(PasswordRequiresUpper), _localizer[nameof(PasswordRequiresUpper)].Value);

  public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
    NewIdentityError(nameof(PasswordRequiresUniqueChars), _localizer[nameof(PasswordRequiresUniqueChars), uniqueChars].Value);

  public override IdentityError PasswordTooShort(int length) =>
    NewIdentityError(nameof(PasswordTooShort), _localizer[nameof(PasswordTooShort), length].Value);

  public override IdentityError UserAlreadyHasPassword() =>
    NewIdentityError(nameof(UserAlreadyHasPassword), _localizer[nameof(UserAlreadyHasPassword)].Value);

  public override IdentityError UserLockoutNotEnabled() =>
    NewIdentityError(nameof(UserLockoutNotEnabled), _localizer[nameof(UserLockoutNotEnabled)].Value);

  private static IdentityError NewIdentityError(string code, string description) =>
    new() { Code = code, Description = description };
}