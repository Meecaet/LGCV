using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("api/MandatTechnologie")]
    public class CVMandatTechnologiesController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public MandatGraphRepository mandatGraphRepository;
        public TechnologieGraphRepository technologieGraphRepository;

        public CVMandatTechnologiesController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            mandatGraphRepository = new MandatGraphRepository();
            technologieGraphRepository = new TechnologieGraphRepository();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Mandat/{mandatId}/AddTechnologie")]
        public ActionResult AddTechnologie(string utilisateurId, string mandatId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandatModel = mandatGraphRepository.GetOne(mandatId);

            var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);
            editionObjectGraphRepository.AjouterNoeud(objetAjoute: technologieModel, viewModelProprieteNom: "Technologies", noeudModifie: mandatModel);

            technologie.GraphId = technologieModel.GraphKey;
            return Json(technologie);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Mandat/{mandatId}/DeleteTechnologie")]
        public ActionResult DeleteTechnologie(string utilisateurId, string mandatId, [FromBody]TechnologieViewModel technologie)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandat = mandatGraphRepository.GetOne(mandatId);

            var technologies = mandat.Projet.Technologies;

            if (technologies.Any(x => x.GraphKey == technologie.GraphId))
            {
                var technologieModel = technologieGraphRepository.GetOne(technologie.GraphId);
                editionObjectGraphRepository.SupprimerNoeud(technologieModel, "Technologies", mandat);
            }
            else
            {
                var edition = mandat.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == technologie.GraphId);
                editionObjectGraphRepository.Delete(edition);
            }

            return Json(technologie);
        }
    }
}