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

        public CVBioController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
        }

        [Route("{cvId}/Detail/{utilisateurId}")]
        [AllowAnonymous]
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
            {
                if (utilisateur.EditionObjects.Count > 0)
                {
                    var edition = utilisateur.EditionObjects.First();
                    editionObjectGraphRepository.Update(edition);
                }
                else
                {
                    var edition = EditionObject.CreateChangementPropriete(proprietesModifiees, utilisateur);
                }
                proprietesModifiees.Clear();
            }

            var cv = utilisateur.Conseiller.CVs.First();
            if (cv.ResumeExperience != bio.Biographie)
            {
                proprietesModifiees.Add(new KeyValuePair<string, string>("ResumeExperience", bio.Biographie));
                var changement = EditionObject.CreateChangementPropriete(proprietesModifiees, cv);
                proprietesModifiees.Clear();
            }

            var conseiller = utilisateur.Conseiller;
            if (conseiller.Fonction.GraphKey != bio.Fonction)
            {
                var changement = EditionObject.CreateChangementRelation(bio.Fonction, conseiller.Fonction.GraphKey, conseiller);
            }

            return Json(new { Status = "OK", Message = "Biographie modifiée" });
        }
    }
}