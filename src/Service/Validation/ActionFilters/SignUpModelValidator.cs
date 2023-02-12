using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service {
  public class SignUpModelValidator : ActionFilterAttribute {
    public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next) {
      SignUpModel signUpModel = RequestModelActionFilterValidatorHelper.GetRequestModel<SignUpModel>(filterContext);

      ValidationResult validationResult = await ValidateSignUpModel(signUpModel);

      await RequestModelActionFilterValidatorHelper.FinalizeActionFilterValidator(validationResult, filterContext, next);
    }

    private async Task<ValidationResult> ValidateSignUpModel(SignUpModel signUpModel) {
      ValidationResult validationResult = new ValidationResult();

      CheckHasDefaultValue(validationResult, signUpModel);
      if (!validationResult.IsValid)
        return validationResult;

      CheckPasswordAvailability(validationResult, signUpModel.Password);
      if (!validationResult.IsValid)
        return validationResult;

      await CheckWhetherUserExist(validationResult, signUpModel.Username);
      if (!validationResult.IsValid)
        return validationResult;

      return validationResult;
    }

    private void CheckHasDefaultValue(ValidationResult validationResult, SignUpModel signUpModel) {
      if (signUpModel is null) {
        validationResult.IsValid = false;
        validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(SignUpModel)}";
      } else if (string.IsNullOrEmpty(signUpModel.Username) || string.IsNullOrWhiteSpace(signUpModel.Username)) {
        validationResult.IsValid = false;
        validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(SignUpModel.Username)}";
      } else if (string.IsNullOrEmpty(signUpModel.Password) || string.IsNullOrWhiteSpace(signUpModel.Password)) {
        validationResult.IsValid = false;
        validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(SignUpModel.Password)}";
      }
    }

    private void CheckPasswordAvailability(ValidationResult validationResult, string password) {
      if (password.Length < 4) {
        validationResult.IsValid = false;
        validationResult.Message = Constants.ValidationMessages.PasswordMustBeMoreThanFourCharacters;
      }
    }

    private async Task CheckWhetherUserExist(ValidationResult validationResult, string userName) {
      validationResult.IsValid = (await Startup.GetInstance<IUserService>().GetUser(u => u.Username == userName)) == null;
      validationResult.Message = validationResult.IsValid ? string.Empty : Constants.ValidationMessages.UserAlreadyExists;
    }
  }
}