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
    [Route("Technologies")]
    public class CVTechnologiesController : CVController
    {
        public TechnologieGraphRepository technologieGraphRepository;

        public CVTechnologiesController() : base()
        {
            technologieGraphRepository = new TechnologieGraphRepository();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var technologies = GetGraphObjects(utilisateur);

            if (technologies.Any(x => x.GraphKey == technologie.GraphId))
                return Json(technologie);

            if(utilisateur.Conseiller.EditionObjects.Any(x => x.ObjetAjoute?.GraphKey == technologie.GraphId))
                return Json(technologie);

            return Json(base.Add(utilisateurId, technologie));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);

            var Technologies = utilisateur.Conseiller.Technologies.ToList();

            if (Technologies.Any(x => x.GraphKey == technologie.GraphId))
            {

                var proprietesModifiees = VerifierProprietesModifiees(technologieModel, technologie);

                if (proprietesModifiees.Count() > 0)
                    editionObjectGraphRepository.CreateOrUpdateProprieteEdition(proprietesModifiees, technologieModel);
            }
            else
            {
                technologieModel.MoisDExperience = technologie.Mois;
                technologieModel.Nom = technologie.Description;

                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == technologie.GraphId);
                technologieGraphRepository.Update(technologieModel, edition);
            }

            return Json(technologie);
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{technologieId}")]
        public new ActionResult Delete(string utilisateurId, string technologieId)
        {
            return Json(base.Delete(utilisateurId, technologieId));
        }


        public override GraphObject CreateGraphObject(ViewModel viewModel)
        {
            var technologie = (TechnologieViewModel)viewModel;
            var technologieModel = technologieGraphRepository.GetOne(viewModel.GraphId);
            technologieModel.MoisDExperience = technologie.Mois;
            return technologieModel;
        }

        public override GraphObject GetGraphObject(string graphId)
        {
            return technologieGraphRepository.GetOne(graphId);
        }

        public override List<GraphObject> GetGraphObjects(Utilisateur utilisateur)
        {
            return utilisateur.Conseiller.Technologies.Cast<GraphObject>().ToList(); ;
        }

        public override List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map)
        {
            return ViewModelFactory<Formation, TechnologieViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: graphObjects,
                map: map);
        }

        public override ViewModel Map(GraphObject graphObject)
        {
            var technologie = (Technologie)graphObject;
            return new TechnologieViewModel
            {
                Mois = technologie.MoisDExperience,
                Description = technologie.Nom,
                GraphId = technologie.GraphKey,
                Categorie = technologie.Categorie?.Nom
            };
        }

        public override List<KeyValuePair<string, string>> VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel)
        {
            var proprietesModifiees = new List<KeyValuePair<string, string>>();
            var technologieObject = (Technologie)graphObject;
            var technologieViewModel = (TechnologieViewModel)viewModel;

            if (technologieObject.MoisDExperience != technologieViewModel.Mois)
                proprietesModifiees.Add(new KeyValuePair<string, string>("Mois", technologieViewModel.Mois.ToString()));

            return proprietesModifiees;
        }

        public override string GetProprieteModifiee()
        {
            return "Technologies";
        }

        public override void UpdateGraphObject(GraphObject graphObject, ViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}