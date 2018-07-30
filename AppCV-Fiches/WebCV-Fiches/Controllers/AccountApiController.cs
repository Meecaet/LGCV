using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebCV_Fiches.Extensions;
using WebCV_Fiches.Models;
using WebCV_Fiches.Models.AccountApiModels;
using WebCV_Fiches.Models.AccountViewModels;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCV_Fiches.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        //ForgotPasswordViewModel
        private ApiCredential apiCredential;


        public AccountApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        [Authorize("Bearer")]
        public IActionResult IsTokenValid()
        {
            return Json(new { ok = true });
        }
        [AllowAnonymous]
        [HttpPost]
        public object DoLogin(
          [FromBody]LoginModel userLogin,
          [FromServices]LoginService login,
          [FromServices]SigningConfigurationsExtensions signingConfigurations,
          [FromServices]TokenConfigurationExtentions tokenConfigurations)
        {
            bool credenciaisValidas = false;
            if (userLogin != null)
            {
                apiCredential   = login.Find(userLogin);
                credenciaisValidas = (apiCredential != null );
            }

            if (credenciaisValidas)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(userLogin.Email, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, userLogin.Email)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromDays(10);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

      
                apiCredential.created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss");
                apiCredential.expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss");
                apiCredential.Token = token;
                return apiCredential;
            }
            else
            {

                apiCredential.authenticated = false;
                apiCredential.message = "User not found";

                return apiCredential;


            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody]RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Nom = model.Nom, Prenom = model.Prenom };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");

                var utilisateurGraphRepository = new UtilisateurGraphRepository();
                var utilisateur = new Utilisateur() {
                    Prenom = user.Prenom,
                    Nom = user.Nom,
                    AdresseCourriel = user.Email
                };
                var savedUserModel = utilisateurGraphRepository.CreateIfNotExists(utilisateur);
                return Json(savedUserModel);
            }
            else
            {
                ErrorViewModel error = new ErrorViewModel();
                error.RequestId = "Le rôle n'a pas pu être modifié";
                return Json(error);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            return View();
        }

       

    }
}

