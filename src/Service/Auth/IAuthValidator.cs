using MessagingService.Common;

namespace MessagingService.Service.Auth {

  public interface IAuthValidator {

    ValidationResult validate(LoginRequest request);

  }

}