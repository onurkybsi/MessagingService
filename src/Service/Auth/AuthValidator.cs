using System;
using System.Text.RegularExpressions;
using MessagingService.Common;

namespace MessagingService.Service.Auth {

  public class AuthValidator : IAuthValidator {

    private static readonly Regex emailRegex = new Regex("^\\S+@\\S+\\.\\S+$");
    private static readonly Regex strongPasswordRegex = new Regex("^\\S+@\\S+\\.\\S+$");

    public ValidationResult validate(LoginRequest request) {
      var result = new ValidationResult();

      validateEmail(request.Email, result);
      validatePassword(request.Password, result);

      return result;
    }

    private static void validateEmail(string email, ValidationResult result) {
      if (string.IsNullOrWhiteSpace(email)) {
        result.AddFailure("email", ValidationFailureMessages.Empty);
        return;
      }
      if (email.Length >= 24) {
        result.AddFailure("email", "cannot be more than 24 characters");
        return;
      }
      if (!emailRegex.IsMatch(email)) {
        result.AddFailure("email", "must be email formatted");
      }
    }

    private void validatePassword(string password, ValidationResult result) {
      if (string.IsNullOrWhiteSpace(password)) {
        result.AddFailure("password", ValidationFailureMessages.Empty);
        return;
      }

      if (password.Length >= 48) {
        result.AddFailure("password", "cannot be more than 48 characters");
        return;
      }

      if (strongPasswordRegex.IsMatch(password)) {
        result.AddFailure("password", "must meet strong password conditions");
      }
    }

  }

}