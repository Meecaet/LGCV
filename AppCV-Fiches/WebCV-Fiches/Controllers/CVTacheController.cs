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
    [Route("api/Tache")]
    public class CVTacheController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public MandatGraphRepository mandatGraphRepository;
        public TacheGraphRepository tacheGraphRepository;

        public CVTacheController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            mandatGraphRepository = new MandatGraphRepository();
            tacheGraphRepository = new TacheGraphRepository();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Mandat/{mandatId}/AddTache")]
        public ActionResult AddTache(string utilisateurId, string mandatId, [FromBody]TacheViewModel tache)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandatModel = mandatGraphRepository.GetOne(mandatId);

            var tacheModel = new Tache { Description = tache.Description };
            tacheGraphRepository.Add(tacheModel);
            editionObjectGraphRepository.AjouterNoeud(objetAjoute: tacheModel, noeudModifiePropriete: "Taches", noeudModifie: mandatModel);

            tache.GraphId = tacheModel.GraphKey;
            return Json(tache);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Mandat/{mandatId}/DeleteTache")]
        public ActionResult DeleteTache(string utilisateurId, string mandatId, [FromBody]TacheViewModel tache)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandat = mandatGraphRepository.GetOne(mandatId);

            var taches = mandat.Taches;

            if (taches.Any(x => x.GraphKey == tache.GraphId))
            {
                var tacheModel = tacheGraphRepository.GetOne(tache.GraphId);
                editionObjectGraphRepository.SupprimerNoeud(tacheModel, "Taches", mandat);
            }
            else
            {
                var edition = mandat.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == tache.GraphId);
                editionObjectGraphRepository.Delete(edition);
            }

            return Json(tache);

        }

    }
}