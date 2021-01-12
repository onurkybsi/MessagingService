using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service
{
    public class LoginModelValidator : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            LoginModel loginModel = filterContext.ActionArguments.Values.FirstOrDefault() as LoginModel;

            ValidationResult validationResult = ValidateLoginModel(loginModel);

            if (!validationResult.IsValid)
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                filterContext.Result = new JsonResult(validationResult);
            }
            else
            { await next(); }
        }

        private ValidationResult ValidateLoginModel(LoginModel loginModel)
        {
            ValidationResult validationResult = new ValidationResult();

            CheckHasDefaultValue(validationResult, loginModel);
            if (!validationResult.IsValid)
                return validationResult;

            CheckPasswordAvailability(validationResult, loginModel.Password);
            if (!validationResult.IsValid)
                return validationResult;

            return validationResult;
        }

        private void CheckHasDefaultValue(ValidationResult validationResult, LoginModel loginModel)
        {
            if (loginModel is null)
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(loginModel)}";
            }
            else if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Username))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(loginModel.Username)}";
            }
            else if (string.IsNullOrEmpty(loginModel.Password) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(loginModel.Password)}";
            }
        }

        private void CheckPasswordAvailability(ValidationResult validationResult, string password)
        {
            if (password.Length < 4)
            {
                validationResult.IsValid = false;
                validationResult.Message = Constants.ValidationMessages.PasswordMustBeMoreThanFourCharacters;
            }
        }
    }
}