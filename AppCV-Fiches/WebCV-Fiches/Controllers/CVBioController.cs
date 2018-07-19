using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Bio")]
    public class CVBioController : Controller
    {
        private UtilisateurGraphRepository UtilisateurDepot;
        private IMemoryCache cache;
        public CVBioController()
        {
            UtilisateurDepot = new UtilisateurGraphRepository();
            this.cache = cache;
        }


        [Route("Detail/{utilisateurId}")]
        //[AllowAnonymous]
        [Authorize("Bearer")]
        public ActionResult Detail(string cvId, string utilisateurId)
        {

            var bio = new BioViewModel();
            Utilisateur utilisateur;

            utilisateur = UtilisateurDepot.GetOne(utilisateurId);

            bio.Nom = utilisateur.Nom ?? "";
            bio.Prenom = utilisateur.Prenom ?? "";
            bio.Biographie = "To implement";

            return Json(bio);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]BioViewModel bio)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Biographie modifiée" });
        }
        [Authorize("Bearer")]
        [HttpPost]
        [Route("Create")]
        public ActionResult Create([FromBody]BioViewModel bio)
        {
            // Objet sugeré comme viewModel.


            bio.GraphIdUtilisateur = "retorno ok";
            return Json(bio);
        }
    }
}