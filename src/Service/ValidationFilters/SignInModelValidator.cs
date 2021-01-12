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
            var signInModel = filterContext.ActionArguments.Values.FirstOrDefault() as LoginModel;

            var user = await Startup.GetInstance<IUserService>().GetUser(u => u.Username == signInModel.Username);

            if (user != null)
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                filterContext.Result = new JsonResult(new { IsValid = false, Message = Constants.ValidationMessages.UserAlreadyExists });
            }
            else
            {
                await next();
            }
        }
    }
}