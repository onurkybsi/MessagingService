using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service
{
    public class SignInModelValidator : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            SignInModel signInModel = filterContext.ActionArguments.Values.FirstOrDefault() as SignInModel;

            ValidationResult validationResult = await ValidateSignInModel(signInModel);

            if (!validationResult.IsValid)
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                filterContext.Result = new JsonResult(validationResult);
            }
            else
            { await next(); }
        }

        private async Task<ValidationResult> ValidateSignInModel(SignInModel signInModel)
        {
            ValidationResult validationResult = new ValidationResult();

            CheckHasDefaultValue(validationResult, signInModel);
            if (!validationResult.IsValid)
                return validationResult;

            CheckPasswordAvailability(validationResult, signInModel.Password);
            if (!validationResult.IsValid)
                return validationResult;

            await CheckWhetherUserExist(validationResult, signInModel.Username);
            if (!validationResult.IsValid)
                return validationResult;

            return validationResult;
        }

        private void CheckHasDefaultValue(ValidationResult validationResult, SignInModel signInModel)
        {
            if (signInModel is null)
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(SignInModel)}";
            }
            else if (string.IsNullOrEmpty(signInModel.Username) || string.IsNullOrWhiteSpace(signInModel.Username))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(signInModel.Username)}";
            }
            else if (string.IsNullOrEmpty(signInModel.Password) || string.IsNullOrWhiteSpace(signInModel.Password))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(signInModel.Password)}";
            }
        }

        private void CheckPasswordAvailability(ValidationResult validationResult, string password)
        {
            if (password.Length < 4)
            {
                validationResult.IsValid = false;
                validationResult.Message = Constants.ValidationMessages.PasswordMustBeMoreThanFourCharacters;
                return;
            }
        }

        private async Task CheckWhetherUserExist(ValidationResult validationResult, string userName)
        {
            validationResult.IsValid = (await Startup.GetInstance<IUserService>().GetUser(u => u.Username == userName)) != null;
            validationResult.Message = validationResult.IsValid ? string.Empty : Constants.ValidationMessages.UserAlreadyExists;
        }
    }
}