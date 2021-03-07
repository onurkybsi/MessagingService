using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service
{
    public class MessageGroupCreationContextValidator : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
            => await RequestModelActionFilterValidatorHelper.CompleteActionFilterValidatorProcess<MessageGroupCreationContext>(new List<Action<MessageGroupCreationContext, ValidationResult>>
            {
                CheckHasDefaultValue
            }, filterContext, next);

        private void CheckHasDefaultValue(MessageGroupCreationContext messageGroupCreationContext, ValidationResult validationResult)
        {
            if (messageGroupCreationContext is null)
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(messageGroupCreationContext)}";
            }
            else if (string.IsNullOrEmpty(messageGroupCreationContext.GroupName) || string.IsNullOrWhiteSpace(messageGroupCreationContext.GroupName))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(messageGroupCreationContext.GroupName)}";
            }
        }
    }
}