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
            var temp = base.All(utilisateurId);
            var orderBy = temp.Cast<PublicationViewModel>().ToList().Where(f =>
                                            f.GraphId != null
                                             ).OrderByDescending(x => x.Annee);
            return Json(orderBy);
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
            var publication = (Formation)graphObject;
            return new PublicationViewModel
            {
                Annee = publication.AnAcquisition,
                Description = publication.Description,
                GraphId = publication.GraphKey,
                GraphIdGenre = publication.Type.GraphKey,
            };
        }

        public override void UpdateGraphObject(GraphObject graphObject, ViewModel viewModel)
        {
            var publicationObject = (Formation)graphObject;
            var publicationViewModel = (PublicationViewModel)viewModel;
            publicationObject.AnAcquisition = publicationViewModel.Annee;
            publicationObject.Description = publicationViewModel.Description;
            formationGraphRepository.Update(publicationObject);
        }

        public override void VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel)
        {
            var publicationObject = (Formation)graphObject;
            var publicationViewModel = (PublicationViewModel)viewModel;

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => publicationViewModel.Annee,
                graphModelPropriete: () => publicationObject.AnAcquisition,
                noeudModifie: graphObject);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => publicationViewModel.Description,
                graphModelPropriete: () => publicationObject.Description,
                noeudModifie: graphObject);
        }
    }
}