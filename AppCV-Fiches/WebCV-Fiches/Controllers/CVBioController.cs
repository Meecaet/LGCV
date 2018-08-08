using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebCV_Fiches.Filters;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("api/Bio")]
    [AuthorizeRoleFilter("Administrateur", "Conseiller")]
    public class CVBioController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public FonctionGraphRepository fonctionGraphRepository;

        public CVBioController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            fonctionGraphRepository = new FonctionGraphRepository();
        }

        [Route("Detail/{utilisateurId}")]
        [Authorize("Bearer")]
        public ActionResult Detail(string utilisateurId)
        {
            var bioViewModel = new BioViewModel();

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            bioViewModel.Nom = utilisateur.Nom;
            bioViewModel.Prenom = utilisateur.Prenom;

            var cv = utilisateur?.Conseiller?.CVs?.First();

            if (cv == null)
                return Json(new { });

            bioViewModel.ResumeExperience = cv.ResumeExperience;

            var conseiller = utilisateur.Conseiller;
            bioViewModel.Fonction = conseiller.Fonction.GraphKey;

            var editions = new EditionObjectViewModelFactory<BioViewModel>();
            bioViewModel.editionObjecViewModels = editions.GetEditions(utilisateur, cv, conseiller);

            var returnJon = new
            {
            };

            return Json(bioViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]BioViewModel bio)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            editionObjectGraphRepository.ChangerPropriete(viewModelPropriete: () => bio.Nom, graphModelPropriete: () => utilisateur.Nom, noeudModifie: utilisateur);
            editionObjectGraphRepository.ChangerPropriete(viewModelPropriete: () => bio.Prenom, graphModelPropriete: () => utilisateur.Prenom, noeudModifie: utilisateur);

            var cv = utilisateur.Conseiller.CVs.First();
            editionObjectGraphRepository.ChangerPropriete(viewModelPropriete: () => bio.ResumeExperience, graphModelPropriete: () => cv.ResumeExperience, noeudModifie: cv);

            var conseiller = utilisateur.Conseiller;
            editionObjectGraphRepository.ChangerPropriete(viewModelPropriete: () => bio.Fonction, graphModelPropriete: () => conseiller.Fonction.GraphKey, graphModelProprieteNom: "Fonction", noeudModifie: cv);

            return Json(new { Status = "OK", Message = "ResumeExperience modifiée" });
        }
    }
}
