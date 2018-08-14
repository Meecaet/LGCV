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
    [Route("api/Technologies")]
    public class CVTechnologiesController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public TechnologieGraphRepository technologieGraphRepository;

        public CVTechnologiesController() : base()
        {
            technologieGraphRepository = new TechnologieGraphRepository();
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
        }

        [Route("{utilisateurId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var tecnologies = utilisateur.Conseiller?.Technologies?.Cast<GraphObject>().ToList();
            if (tecnologies == null)
                return Json(new List<TechnologieViewModel>());
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(tecnologies);
            var tecnologiesViewModel = ViewModelFactory<Technologie, TechnologieViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudModifie,
                graphObjects: tecnologies,
                map: Map);

            if (tecnologiesViewModel.Count > 0)
            {

                List<object> newJson = new List<object>();

                var allCategories = tecnologiesViewModel.Select(s => (s as TechnologieViewModel).Categorie).Distinct().OrderBy(o => o).ToList();

                allCategories.ForEach(f =>
                {
                    newJson.Add(new
                    {
                        categorie = f,
                        dataByCategorie = tecnologiesViewModel.Where(w => (w as TechnologieViewModel).Categorie == f).ToList()
                    });
                });

                return Json(newJson);
            }
            else
            {
                return Json(new List<TechnologieViewModel>());
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody] List<TechnologieViewModel> technologies)
        {
            try
            {
                foreach (var technologie in technologies)
                {
                    var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
                    var _technologies = utilisateur.Conseiller.Technologies;

                    if (_technologies.Any(x => x.GraphKey == technologie.GraphId))
                        continue;

                    if (utilisateur.Conseiller.EditionObjects.Any(x => x.ObjetAjoute?.GraphKey == technologie.GraphId))
                        continue;

                    var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);
                    technologieModel.MoisDExperience = technologie.Mois;

                    editionObjectGraphRepository.AjouterNoeud(
                        objetAjoute: technologieModel,
                        viewModelProprieteNom: "Technologies",
                        noeudModifie: utilisateur.Conseiller);

                    technologie.GraphId = technologieModel.GraphKey;
                }
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var Technologies = utilisateur.Conseiller.Technologies;

            if (Technologies.Any(x => x.GraphKey == technologie.GraphId))
            {
                var technologieModel = Technologies.Find(x => x.GraphKey == technologie.GraphId);
                editionObjectGraphRepository.ChangerPropriete(
                    viewModelPropriete: () => technologie.Mois,
                    graphModelPropriete: () => technologieModel.MoisDExperience,
                    noeudModifie: technologieModel);
            }
            else
            {
                var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);
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
        public ActionResult Delete(string utilisateurId, string technologieId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var technologie = technologieGraphRepository.GetOne(technologieId);

            var technologies = utilisateur.Conseiller.Technologies;

            if (technologies.Any(x => x.GraphKey == technologie.GraphKey))
            {
                foreach (var edition in technologie.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(technologie, "Technologies", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == technologie.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }

            return NoContent();
        }


        public ViewModel Map(GraphObject graphObject)
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
    }
}