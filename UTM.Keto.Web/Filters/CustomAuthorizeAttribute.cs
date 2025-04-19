using System;
using System.Web;
using System.Web.Mvc;

namespace UTM.Keto.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // User is not authenticated, redirect to login page
                filterContext.Result = new RedirectResult("~/Auth/Login");
            }
            else
            {
                // User is authenticated but not authorized (wrong role)
                filterContext.Result = new RedirectResult("~/Home/AccessDenied");
            }
        }
    }
} 