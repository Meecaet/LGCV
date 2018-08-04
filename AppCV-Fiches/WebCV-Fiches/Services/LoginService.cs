using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using WebCV_Fiches.Models;
using WebCV_Fiches.Models.AccountApiModels;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private UtilisateurGraphRepository UtilisateurDepot;
        public LoginService(SignInManager<ApplicationUser> signInManager)
        {
            UtilisateurDepot = new UtilisateurGraphRepository();
            _signInManager = signInManager;
        }

        public ApiCredential Find(LoginModel loginModel, string NomeComplet)
        {
            ApiCredential credential = new ApiCredential();

            var result = _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RemeberMe, lockoutOnFailure: false);

            if (result.Result == SignInResult.Success)
            {
                //#if !debug
                //  var utilisateur = UtilisateurDepot.Search(new Dictionary<string, object> { { "Nom", "MATEUS ELOY EVANGELISTA CAETANO" } }).DefaultIfEmpty(null).FirstOrDefault();

                //#else
                var utilisateur = UtilisateurDepot.Search(new Dictionary<string, object> { { "AdresseCourriel", loginModel.Email } }).DefaultIfEmpty(null).FirstOrDefault();
                if (utilisateur == null)
                {
                    utilisateur = UtilisateurDepot.Search(new Dictionary<string, object> { { "Nom", NomeComplet.ToUpper() } }).DefaultIfEmpty(null).FirstOrDefault();
                    utilisateur.AdresseCourriel = loginModel.Email;
                    var repository = new UtilisateurGraphRepository();
                    repository.Update(utilisateur);
                }
                //#endif

                credential.authenticated = true;
                credential.message = "Ok";
                credential.utilisateurId = utilisateur.GraphKey;
            }
            return credential;
        }


    }
}
