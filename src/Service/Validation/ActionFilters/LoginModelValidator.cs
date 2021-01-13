using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service
{
    public class LoginModelValidator : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
            => await RequestModelActionFilterValidatorHelper.CompleteActionFilterValidatorProcess<LoginModel>(new List<Action<LoginModel, ValidationResult>>
            {
                CheckHasDefaultValue, CheckPasswordAvailability
            }, filterContext, next);

        private void CheckHasDefaultValue(LoginModel loginModel, ValidationResult validationResult)
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

        private void CheckPasswordAvailability(LoginModel loginModel, ValidationResult validationResult)
        {
            if (loginModel.Password.Length < 4)
            {
                validationResult.IsValid = false;
                validationResult.Message = Constants.ValidationMessages.PasswordMustBeMoreThanFourCharacters;
            }
        }
    }
}