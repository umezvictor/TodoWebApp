using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Client.Services;

namespace Web.Client.ActionFilters
{
    //this class checks if a user is logged in, before granting access to any resource
    //apply it at controller level
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private readonly ITokenService tokenService;

        public AuthenticationFilter(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //var result = context.Result;
            //Do something with Result.
            if (context.Canceled == true)
            {
                //Action execution was short-circuited by another filter.
            }

            if (context.Exception != null)
            {
                //Exception thrown by action or action filter.
                //Set to null to handle the exception.
                context.Exception = null;
            }
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //get token from session
            var token = context.HttpContext.Session.GetString("token");
            //validate token
            bool isTokenValid = tokenService.ValidateJwtToken(token);
            if (!isTokenValid)
            {
                //redirect to login page if token is invalid
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Auth" }, { "action", "Login" } });

            }

        }

    }
}
