using MessagingService.Common;

namespace MessagingService.Service.User {

  internal class UserValidator {

    public ValidationResult Validate(User user) {
      ValidationResult result = new ValidationResult();

      if (!ValidationUtils.IsEmail(user.Email)) {
        result.AddFailure("email", "must be email formatted");
      }

      return result;
    }

  }

}