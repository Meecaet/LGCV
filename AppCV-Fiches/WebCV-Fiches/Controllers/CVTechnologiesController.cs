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
            if(tecnologies == null)    
                return Json(new List<TechnologieViewModel>());
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(tecnologies);
            var tecnologiesViewModel = ViewModelFactory<Technologie, TechnologieViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudModifie,
                graphObjects: tecnologies,
                map: Map);

            return Json(tecnologiesViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var technologies = utilisateur.Conseiller.Technologies;

            if (technologies.Any(x => x.GraphKey == technologie.GraphId))
                return Json(technologie);

            if (utilisateur.Conseiller.EditionObjects.Any(x => x.ObjetAjoute?.GraphKey == technologie.GraphId))
                return Json(technologie);

            var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);
            technologieModel.MoisDExperience = technologie.Mois;

            editionObjectGraphRepository.AjouterNoeud(objetAjoute: technologieModel, noeudModifiePropriete: "Technologies", noeudModifie: utilisateur.Conseiller);

            technologie.GraphId = technologieModel.GraphKey;

            return Json(technologieModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);

            var Technologies = utilisateur.Conseiller.Technologies;

            if (Technologies.Any(x => x.GraphKey == technologie.GraphId))
            {
                var proprietesModifiees = new List<KeyValuePair<string, string>>();
                
                if (technologieModel.MoisDExperience != technologie.Mois)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("Mois", technologie.Mois.ToString()));

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