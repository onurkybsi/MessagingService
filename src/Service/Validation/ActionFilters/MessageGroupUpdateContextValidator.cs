using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service
{
    public class MessageGroupUpdateContextValidator : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
            => await RequestModelActionFilterValidatorHelper.CompleteActionFilterValidatorProcess<MessageGroupUpdateContext>(new List<Action<MessageGroupUpdateContext, ValidationResult>>
            {
                CheckHasDefaultValue
            }, filterContext, next);

        private void CheckHasDefaultValue(MessageGroupUpdateContext messageGroupUpdateContext, ValidationResult validationResult)
        {
            if (messageGroupUpdateContext is null)
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(messageGroupUpdateContext)}";
            }
            else if (string.IsNullOrEmpty(messageGroupUpdateContext.MessageGroupId) || string.IsNullOrWhiteSpace(messageGroupUpdateContext.MessageGroupId))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(messageGroupUpdateContext.MessageGroupId)}";
            }
            else if (string.IsNullOrEmpty(messageGroupUpdateContext.Username) || string.IsNullOrWhiteSpace(messageGroupUpdateContext.Username))
            {
                validationResult.IsValid = false;
                validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(messageGroupUpdateContext.Username)}";
            }
        }
    }
}