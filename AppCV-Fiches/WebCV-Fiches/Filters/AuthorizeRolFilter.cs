using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Filters
{

    public class AuthorizeRoleFilter : Attribute, IActionFilter
    {
        private readonly string[] roles;

        public AuthorizeRoleFilter(params string[] roles)
        {
            this.roles = roles;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            var email = context.HttpContext.User.Identity.Name;
            UserManager<ApplicationUser> applicationUser = (UserManager<ApplicationUser>)context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = applicationUser.FindByEmailAsync(context.HttpContext.User.Identity.Name).Result;
            if (applicationUser.IsEmailConfirmedAsync(user).Result)
            {
                foreach (string role in roles)
                {
                    if (applicationUser.IsInRoleAsync(user, role).Result)
                    {
                        return;
                    }
                }
            }
            else
            {
                context.HttpContext.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes("Email non confirmé"));
            }
            context.HttpContext.Response.StatusCode = 403;
        }

    }

}
