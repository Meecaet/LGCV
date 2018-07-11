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
        public ChangementGraphRepository changementGraphRepository;
        public ChangementRelationGraphRepository changementRelationGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;

        public CVBioController()
        {
            changementGraphRepository = new ChangementGraphRepository();
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            changementRelationGraphRepository = new ChangementRelationGraphRepository();
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
            List<ProprieteModifiee> proprietesModifiees = new List<ProprieteModifiee>();

            if (utilisateur.Prenom != bio.Prenom)
                proprietesModifiees.Add(new ProprieteModifiee() { Nom = "Prenom", valeur = bio.Prenom });

            if (utilisateur.Nom != bio.Nom)
                proprietesModifiees.Add(new ProprieteModifiee() { Nom = "Nom", valeur = bio.Nom });

            if (proprietesModifiees.Count() > 0)
            {
                var changement = new Changement();
                changement.NoeudModifie = utilisateur;
                changement.ProprietesModifiees.AddRange(proprietesModifiees);
                //changementGraphRepository.Add(changement);
                proprietesModifiees.Clear();
            }

            var cv = utilisateur.Conseiller.CVs.First();
            if (cv.ResumeExperience != bio.Biographie)
            {
                proprietesModifiees.Add(new ProprieteModifiee() { Nom = "ResumeExperience", valeur = bio.Prenom });
                var changement = new Changement();
                changement.NoeudModifie = cv;
                changement.ProprietesModifiees.AddRange(proprietesModifiees);
                //changementGraphRepository.Add(changement);
                proprietesModifiees.Clear();
            }

            var conseiller = utilisateur.Conseiller;
            if (conseiller.Fonction.GraphKey != bio.Fonction)
            {
                var changement = new ChangementRelation();
                changement.NoeudModifie = conseiller;
                changement.ObjetAjouteId = bio.Fonction;
                changement.ObjetSupprimeId = conseiller.Fonction.GraphKey;
                changementRelationGraphRepository.Add(changement);
            }

            return Json(new { Status = "OK", Message = "Biographie modifiée" });
        }
    }
}