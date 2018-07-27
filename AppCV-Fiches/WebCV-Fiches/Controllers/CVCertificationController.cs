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
    [Route("Certification")]
    public class CVCertificationController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public FormationGraphRepository formationGraphRepository;

        public CVCertificationController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            formationGraphRepository = new FormationGraphRepository();
        }

        [Route("{utilisateurId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {
            Func<GraphObject, ViewModel> map = this.map;

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var certifications = utilisateur.Conseiller.Certifications().Cast<GraphObject>().ToList(); ;
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(certifications);
            var certificationsViewModel = ViewModelFactory<Formation,CertificationViewModel>.GetViewModels(utilisateurId: utilisateurId, noeudsModifie: noeudModifie, graphObjects: certifications, map: map);
            return Json(certificationsViewModel);
        }

        private ViewModel map(GraphObject certificationModel)
        {
            var certification = (Formation)certificationModel;
            return new CertificationViewModel
            {
                Annee = certification.AnAcquisition,
                Description = certification.Description,
                GraphId = certification.GraphKey,
                GraphIdGenre = certification.Type.GraphKey,
            };
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]CertificationViewModel certification)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var certificationModel = Formation.CreateCertification(certification.Annee, certification.Description);
            formationGraphRepository.Add(certificationModel);
            editionObjectGraphRepository.AjouterNoeud(objetAjoute: certificationModel, noeudModifiePropriete: "Formations", noeudModifie: utilisateur.Conseiller);

            certification.GraphId = certificationModel.GraphKey;
            certification.GraphIdGenre = certificationModel.Type.GraphKey;

            return Json(certification);

        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]CertificationViewModel certification)
        {

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var certificationModel = formationGraphRepository.GetOne(certification.GraphId);

            var certifications = utilisateur.Conseiller.Certifications();

            if (certifications.Any(x => x.GraphKey == certification.GraphId))
            {

                var proprietesModifiees = new List<KeyValuePair<string, string>>();

                if (certificationModel.AnAcquisition != certification.Annee)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("Annee", certification.Annee.ToString()));

                if (certificationModel.Description != certification.Description)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("Description", certification.Description));

                if (proprietesModifiees.Count() > 0)
                    editionObjectGraphRepository.CreateOrUpdateProprieteEdition(proprietesModifiees, certificationModel);
            }
            else
            {
                certificationModel.AnAcquisition = certification.Annee;
                certificationModel.Description = certification.Description;
                formationGraphRepository.Update(certificationModel);
            }

            return Json(certification);
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{certificationId}")]
        public ActionResult Delete(string utilisateurId, string certificationId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var certification = formationGraphRepository.GetOne(certificationId);


            var certifications = utilisateur.Conseiller.Certifications();

            if (certifications.Any(x => x.GraphKey == certification.GraphKey))
            {
                foreach (var edition in certification.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(certification, "Formations", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute.GraphKey == certification.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }



            return Json(certification);
        }
    }
}
