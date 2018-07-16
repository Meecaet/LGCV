using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Bio")]
    public class CVBioController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public ProprieteModifieeGraphRepository proprieteModifieeGraphRepository;

        public CVBioController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            proprieteModifieeGraphRepository = new ProprieteModifieeGraphRepository();
        }

        [Route("{cvId}/Detail/{utilisateurId}")]
        //[AllowAnonymous]
        [Authorize("Bearer")]
        public ActionResult Detail(string cvId, string utilisateurId)
        {
            return Json(new BioViewModel());
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]BioViewModel bio)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            List<KeyValuePair<string, string>> proprietesModifiees = new List<KeyValuePair<string, string>>();

            if (utilisateur.Prenom != bio.Prenom)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Prenom", bio.Prenom));

            if (utilisateur.Nom != bio.Nom)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Nom", bio.Nom));

            if (proprietesModifiees.Count() > 0)
                editionObjectGraphRepository.CreateOrUpdateChangementPropietes(proprietesModifiees, utilisateur);

            var cv = utilisateur.Conseiller.CVs.First();
            if (cv.ResumeExperience != bio.Biographie)
            {
                proprietesModifiees.Clear();
                proprietesModifiees.Add(new KeyValuePair<string, string>("ResumeExperience", bio.Biographie));
                editionObjectGraphRepository.CreateOrUpdateChangementPropietes(proprietesModifiees, cv);
            }

            var conseiller = utilisateur.Conseiller;
            if (conseiller.Fonction.GraphKey != bio.Fonction)
                editionObjectGraphRepository.CreateOrUpdateChangementRelation(bio.Fonction, conseiller.Fonction.GraphKey, conseiller);

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