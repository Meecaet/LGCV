using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("api/Perfectionnement")]
    public class CVPerfectionnementController : CVFormation
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
        public ActionResult Add(string utilisateurId, [FromBody]PerfectionnementViewModel perfectionnement)
        {
            return Json(base.Add(utilisateurId, perfectionnement));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]PerfectionnementViewModel perfectionnement)
        {
            return Json(base.Edit(utilisateurId, perfectionnement));
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{perfectionnementId}")]
        public new ActionResult Delete(string utilisateurId, string perfectionnementId)
        {
            return Json(base.Delete(utilisateurId, perfectionnementId));
        }

        public override GraphObject CreateGraphObject(ViewModel viewModel)
        {
            var perfectionnement = (PerfectionnementViewModel)viewModel;
            var PerfectionnementModel = Formation.CreateFormation(perfectionnement.Annee, perfectionnement.Description, FormationType.Perfectionnement);
            formationGraphRepository.Add(PerfectionnementModel);
            return PerfectionnementModel;
        }

        public override List<GraphObject> GetGraphObjects(Utilisateur utilisateur)
        {
            return utilisateur.Conseiller.Perfectionnement().Cast<GraphObject>().ToList(); ;
        }

        public override List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map)
        {
            return ViewModelFactory<Formation, PerfectionnementViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: graphObjects,
                map: map);
        }

        public override ViewModel Map(GraphObject graphObject)
        {
            var perfectionnement = (Formation)graphObject;
            return new PerfectionnementViewModel
            {
                Annee = perfectionnement.AnAcquisition,
                Description = perfectionnement.Description,
                GraphId = perfectionnement.GraphKey,
                GraphIdGenre = perfectionnement.Type.GraphKey,
            };
        }

        public override void UpdateGraphObject(GraphObject graphObject, ViewModel viewModel)
        {
            var perfectionnementObject = (Formation)graphObject;
            var perfectionnementViewModel = (PerfectionnementViewModel)viewModel;
            perfectionnementObject.AnAcquisition = perfectionnementViewModel.Annee;
            perfectionnementObject.Description = perfectionnementViewModel.Description;
            formationGraphRepository.Update(perfectionnementObject);
        }

        public override void VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel)
        {
            var perfectionnementObject = (Formation)graphObject;
            var perfectionnementViewModel = (PerfectionnementViewModel)viewModel;

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => perfectionnementViewModel.Annee,
                graphModelPropriete: () => perfectionnementObject.AnAcquisition,
                noeudModifie: graphObject);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => perfectionnementViewModel.Description,
                graphModelPropriete: () => perfectionnementObject.Description,
                noeudModifie: graphObject);
        }
    }
}