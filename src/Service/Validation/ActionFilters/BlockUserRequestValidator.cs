using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingService.Service {
  public class BlockUserRequestValidator : ActionFilterAttribute {
    public async override Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        => await RequestModelActionFilterValidatorHelper.CompleteActionFilterValidatorProcess<BlockUserRequest>(new List<Action<BlockUserRequest, ValidationResult>>
        {
                CheckHasDefaultValue
        }, filterContext, next);


    private void CheckHasDefaultValue(BlockUserRequest blockUserRequest, ValidationResult validationResult) {
      if (blockUserRequest is null) {
        validationResult.IsValid = false;
        validationResult.Message = $"{Constants.ValidationMessages.ValueCanNotBeNull}: {nameof(blockUserRequest)}";
      } else if (string.IsNullOrEmpty(blockUserRequest.BlockedUsername) || string.IsNullOrWhiteSpace(blockUserRequest.BlockedUsername)) {
        validationResult.IsValid = false;
        validationResult.Message = $"{Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace}: {nameof(blockUserRequest.BlockedUsername)}";
      }
    }
  }
}