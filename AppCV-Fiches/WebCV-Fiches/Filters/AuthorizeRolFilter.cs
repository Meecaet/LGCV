using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Filters
{

    public class AuthorizeRoleFilter : Attribute, IActionFilter
    {
        private string role;

        public AuthorizeRoleFilter(string role) {
            this.role = role;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var email = context.HttpContext.User.Identity.Name;
            UserManager<ApplicationUser> applicationUser = (UserManager<ApplicationUser>)context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = applicationUser.FindByEmailAsync(email).Result;
            if (!applicationUser.IsInRoleAsync(user, role).Result)
            {
                context.HttpContext.Response.StatusCode = 403;
            }

        }

    }

}
