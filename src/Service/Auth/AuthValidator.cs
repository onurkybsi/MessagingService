using System;
using System.Text.RegularExpressions;
using MessagingService.Common;

namespace MessagingService.Service.Auth {

  internal class AuthValidator {

    private static readonly Regex strongPasswordRegex = new Regex("^\\S+@\\S+\\.\\S+$");

    public ValidationResult validate(LoginRequest request) {
      var result = new ValidationResult();

      ValidateEmail(request.Email, result);
      ValidatePassword(request.Password, result);

      return result;
    }

    private static void ValidateEmail(string email, ValidationResult result) {
      if (ValidationUtils.IsEmail(email)) {
        result.AddFailure("email", "must be email formatted");
      }
    }

    private static void ValidatePassword(string password, ValidationResult result) {
      if (ValidationUtils.IsEmail(password)) {
        result.AddFailure("password", "must be strong");
      }
    }

  }

}