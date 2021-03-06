﻿using System;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebCV_Fiches.Extensions;
using WebCV_Fiches.Filters;
using WebCV_Fiches.Models;
using WebCV_Fiches.Models.AccountApiModels;
using WebCV_Fiches.Models.AccountViewModels;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCV_Fiches.Controllers
{
    [Route("api/AccountApi")]
    public class AccountApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        //ForgotPasswordViewModel
        private ApiCredential apiCredential;


        public AccountApiController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountApiController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize("Bearer")]
        [AuthorizeRoleFilter("Administrateur", "Conseiller", "Approbateur")]
        [Route("IsTokenValid")]
        public IActionResult IsTokenValid()
        {
            return Json(new { ok = true });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("DoLogin")]
        public object DoLogin(
          [FromBody]LoginModel userLogin,
          [FromServices]LoginService login,
          [FromServices]SigningConfigurationsExtensions signingConfigurations,
          [FromServices]TokenConfigurationExtentions tokenConfigurations)
        {
            bool credenciaisValidas = false;
            apiCredential = new ApiCredential();
            if (userLogin == null || userLogin.Email == null)
            {
                apiCredential.authenticated = false;
                apiCredential.message = "User not found";
                return apiCredential;
            }

            var user = _userManager.FindByEmailAsync(userLogin.Email).Result;
            if (user == null)
            {
                apiCredential.authenticated = false;
                apiCredential.message = "User not found";
                return apiCredential;
            }
            apiCredential = login.Find(user, userLogin, $"{user.Prenom} {user.Nom}");
            credenciaisValidas = (apiCredential != null);

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



                var approbateurs = _userManager.GetUsersInRoleAsync(_roleManager.FindByNameAsync("Approbateur").Result.Name).Result.ToList();
                var conseillers = _userManager.GetUsersInRoleAsync(_roleManager.FindByNameAsync("Conseiller").Result.Name).Result.ToList();
                var administrateurs = _userManager.GetUsersInRoleAsync(_roleManager.FindByNameAsync("Administrateur").Result.Name).Result.ToList();

                apiCredential.isAdministrateur = administrateurs.Exists(x => x.Id == user.Id);
                apiCredential.isApprobateur = approbateurs.Exists(x => x.Id == user.Id);
                apiCredential.isConseiller = conseillers.Exists(x => x.Id == user.Id);
                apiCredential.userName = userLogin.Email;
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
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody]RegisterViewModel model)
        {
            var email = $"{model.Email}@lgs.com";
            var user = new ApplicationUser { UserName = email, Email = email, Nom = model.Nom, Prenom = model.Prenom };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                ApplicationRole role = _roleManager.FindByNameAsync("Conseiller").Result;
                await _userManager.AddToRoleAsync(user, role.Name);

                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = System.Net.WebUtility.UrlEncode(code);
                var callbackUrl = $"{_configuration["FrontendBaseUrl"]}/account/confirmemail?userId={user.Id}&code={code}";
                await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");

                var utilisateurGraphRepository = new UtilisateurGraphRepository();
                var utilisateur = new Utilisateur()
                {
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


        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok();
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = System.Net.WebUtility.UrlEncode(code);
            var callbackUrl = $"{_configuration["FrontendBaseUrl"]}/account/resetpassword?code={code}";
            await _emailSender.SendEmailAsync(model.Email, "Réinitialiser le mot de passe",
               $"Veuillez réinitialiser votre mot de passe en cliquant ici: <a href='{callbackUrl}'>link</a>");
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return StatusCode(500);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500, result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmail confirmEmail)
        {
            if (confirmEmail.Code == null)
            {
                return StatusCode(500, new { message = "Invalid Code" });
            }
            var user = await _userManager.FindByIdAsync(confirmEmail.UserId);
            if (user == null)
            {
                return StatusCode(500, $"Unable to load user with ID '{confirmEmail.UserId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmail.Code);
            if (result.Succeeded)
                return Ok();
            else
                return StatusCode(500);
        }

    }
}

