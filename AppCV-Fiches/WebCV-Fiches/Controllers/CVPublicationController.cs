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
    [Route("api/Publication")]
    public class CVPublicationController : CVFormation
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
        public ActionResult Add(string utilisateurId, [FromBody]PublicationViewModel publication)
        {
            return Json(base.Add(utilisateurId, publication));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]PublicationViewModel publication)
        {
            return Json(base.Edit(utilisateurId, publication));
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{publicationId}")]
        public new ActionResult Delete(string utilisateurId, string publicationId)
        {
            return Json(base.Delete(utilisateurId, publicationId));
        }

        public override GraphObject CreateGraphObject(ViewModel viewModel)
        {
            var publication = (PublicationViewModel)viewModel;
            var publicationModel = Formation.CreateFormation(publication.Annee, publication.Description, FormationType.Publication);
            formationGraphRepository.Add(publicationModel);
            return publicationModel;
        }

        public override List<GraphObject> GetGraphObjects(Utilisateur utilisateur)
        {
            return utilisateur.Conseiller.Publication().Cast<GraphObject>().ToList(); ;
        }

        public override List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map)
        {
            return ViewModelFactory<Formation, PublicationViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: graphObjects,
                map: map);
        }

        public override ViewModel Map(GraphObject graphObject)
        {
            var perfectionnement = (Formation)graphObject;
            return new PublicationViewModel
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
            var perfectionnementViewModel = (PublicationViewModel)viewModel;
            perfectionnementObject.AnAcquisition = perfectionnementViewModel.Annee;
            perfectionnementObject.Description = perfectionnementViewModel.Description;
            formationGraphRepository.Update(perfectionnementObject);
        }

        public override List<KeyValuePair<string, string>> VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel)
        {
            var proprietesModifiees = new List<KeyValuePair<string, string>>();
            var perfectionnementObject = (Formation)graphObject;
            var perfectionnementViewModel = (PublicationViewModel)viewModel;

            if (perfectionnementObject.AnAcquisition != perfectionnementViewModel.Annee)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Annee", perfectionnementViewModel.Annee.ToString()));

            if (perfectionnementObject.Description != perfectionnementViewModel.Description)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Description", perfectionnementViewModel.Description));

            return proprietesModifiees;
        }
    }
}