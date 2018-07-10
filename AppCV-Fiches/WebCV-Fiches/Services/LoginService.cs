using Microsoft.AspNetCore.Identity;
using WebCV_Fiches.Models.AccountApiModels;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginService(SignInManager<ApplicationUser> signInManager)
        {

            _signInManager = signInManager;
        }

        public LoginModel Find(LoginModel loginModel)
        {
            LoginModel model = new LoginModel();

            var result = _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RemeberMe, lockoutOnFailure: false);
            if (result.Result == SignInResult.Success)
            {
                model = loginModel;
                model.Id = result.Id;
            }
            return model;
        }

        
    }
}
