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
    [Route("api/CVDomainIntervention")]
    public class CVDomainInterventionController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public DomaineDInterventionGraphRepository domaineDInterventionGraphRepository;

        public CVDomainInterventionController()
        {
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            domaineDInterventionGraphRepository = new DomaineDInterventionGraphRepository();
        }

        [Route("{utilisateurId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {
            Func<GraphObject, ViewModel> map = this.map;

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var domains = utilisateur.Conseiller?.DomaineDInterventions;
            if(domains == null)    
                return Json(new List<DomaineDInterventionViewModel>());

            var domainsObjects = domains.Cast<GraphObject>().ToList();
            var domainsViewModel = ViewModelFactory<DomaineDIntervention, DomaineDInterventionViewModel>.GetViewModels(
                utilisateurId: utilisateurId, 
                noeudsModifie: new List<GraphObject> { utilisateur.Conseiller }, 
                graphObjects: domainsObjects, 
                map: map);

            return Json(domainsViewModel);
        }

        private ViewModel map(GraphObject domainModel)
        {
            var domain = (DomaineDIntervention)domainModel;
            return new DomaineDInterventionViewModel { Description = domain.Description, GraphId = domain.GraphKey };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]DomaineDInterventionViewModel domain)
        // POST: Mandat/Create
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var domainModel = new DomaineDIntervention() { Description = domain.Description };
            var savedDomainModel = domaineDInterventionGraphRepository.CreateIfNotExists(domainModel);
            editionObjectGraphRepository.AjouterNoeud(
                objetAjoute: savedDomainModel, 
                viewModelProprieteNom: "DomaineDInterventions", 
                noeudModifie: utilisateur.Conseiller);

            domain.GraphId = savedDomainModel.GraphKey;
            return Json(domain);
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{domainId}")]
        public ActionResult Delete(string utilisateurId, string domainId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var domain = domaineDInterventionGraphRepository.GetOne(domainId);

            if (utilisateur.Conseiller.DomaineDInterventions.Any(x => x.GraphKey == domain.GraphKey))
            {
                foreach (var edition in domain.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }

                editionObjectGraphRepository.SupprimerNoeud(domain, "DomaineDInterventions", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute.GraphKey == domain.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }

            return Json(domain);
        }
    }
}