using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    public abstract class CVFormation : Controller
    {

        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public FormationGraphRepository formationGraphRepository;

        public CVFormation()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            formationGraphRepository = new FormationGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
        }

        public List<ViewModel> All(string utilisateurId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var graphObjects = GetGraphObjects(utilisateur);
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(graphObjects);
            var graphObjectsViewModel = GetViewModels(utilisateur.GraphKey, noeudModifie, graphObjects, Map);

            return graphObjectsViewModel;
        }

        public ViewModel Add(string utilisateurId, ViewModel viewModel)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var graphObject = CreateGraphObject(viewModel);
            editionObjectGraphRepository.AjouterNoeud(objetAjoute: graphObject, noeudModifiePropriete: GetProprieteModifiee(), noeudModifie: utilisateur.Conseiller);

            viewModel.GraphId = graphObject.GraphKey;

            return viewModel;
        }


        public ViewModel Edit(string utilisateurId, ViewModel viewModel)
        {

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var graphObject = GetGraphObject(viewModel.GraphId);

            var graphObjects = GetGraphObjects(utilisateur);

            if (graphObjects.Any(x => x.GraphKey == viewModel.GraphId))
            {

                var proprietesModifiees = VerifierProprietesModifiees(graphObject, viewModel);

                if (proprietesModifiees.Count() > 0)
                    editionObjectGraphRepository.CreateOrUpdateProprieteEdition(proprietesModifiees, graphObject);
            }
            else
            {
                UpdateGraphObject(graphObject, viewModel);
            }

            return viewModel;
        }

        public GraphObject Delete(string utilisateurId, string viewModelId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var graphObject = GetGraphObject(viewModelId);

            var graphObjects = GetGraphObjects(utilisateur);

            if (graphObjects.Any(x => x.GraphKey == graphObject.GraphKey))
            {
                foreach (var edition in graphObject.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(graphObject, GetProprieteModifiee(), utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == graphObject.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }

            return graphObject;
        }

        public string GetProprieteModifiee()
        {
            return "Formations";
        }

        public  GraphObject GetGraphObject(string graphId)
        {
            return formationGraphRepository.GetOne(graphId);
        }


        public abstract ViewModel Map(GraphObject graphObject);

        public abstract List<GraphObject> GetGraphObjects(Utilisateur utilisateur);

        public abstract List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map);

        public abstract GraphObject CreateGraphObject(ViewModel viewModel);

        public abstract  List<KeyValuePair<string, string>> VerifierProprietesModifiees(GraphObject graphObject, ViewModel viewModel);

        public abstract void UpdateGraphObject(GraphObject graphObject, ViewModel viewModel);
    }
}