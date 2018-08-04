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
    public class CVCertificationController : CVFormation
    {
        [AllowAnonymous]
        [Route("{utilisateurId}/All")]
        public new ActionResult All(string utilisateurId)
        {
            return Json(base.All(utilisateurId));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]CertificationViewModel certification)
        {
            return Json(base.Add(utilisateurId, certification));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]CertificationViewModel certification)
        {
            return Json(base.Edit(utilisateurId, certification));
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{certificationId}")]
        public new ActionResult Delete(string utilisateurId, string certificationId)
        {
            return Json(base.Delete(utilisateurId, certificationId));
        }

        public override GraphObject CreateGraphObject(ViewModel viewModel)
        {
            var certification = (CertificationViewModel)viewModel;
            var certificationModel = Formation.CreateFormation(certification.Annee, certification.Description, FormationType.Certification);
            formationGraphRepository.Add(certificationModel);
            return certificationModel;
        }

        public override List<GraphObject> GetGraphObjects(Utilisateur utilisateur)
        {
            return utilisateur.Conseiller?.Certifications()?.Cast<GraphObject>().ToList(); ;
        }

        public override List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map)
        {
            return ViewModelFactory<Formation, CertificationViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: graphObjects,
                map: map);
        }

        public override ViewModel Map(GraphObject graphObject)
        {
            var perfectionnement = (Formation)graphObject;
            return new CertificationViewModel
            {
                Annee = perfectionnement.AnAcquisition,
                Description = perfectionnement.Description,
                GraphId = perfectionnement.GraphKey,
                GraphIdGenre = perfectionnement.Type.GraphKey,
            };
        }

        public override void UpdateGraphObject(GraphObject graphObject, ViewModel viewModel)
        {
            var certificationObject = (Formation)graphObject;
            var certificationViewModel = (CertificationViewModel)viewModel;
            certificationObject.AnAcquisition = certificationViewModel.Annee;
            certificationObject.Description = certificationViewModel.Description;
            formationGraphRepository.Update(certificationObject);
        }

        public override List<KeyValuePair<string, string>> VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel)
        {
            var proprietesModifiees = new List<KeyValuePair<string, string>>();
            var certificationObject = (Formation)graphObject;
            var certificationViewModel = (CertificationViewModel)viewModel;

            if (certificationObject.AnAcquisition != certificationViewModel.Annee)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Annee", certificationViewModel.Annee.ToString()));

            if (certificationObject.Description != certificationViewModel.Description)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Description", certificationViewModel.Description));

            return proprietesModifiees;
        }
    }
}
