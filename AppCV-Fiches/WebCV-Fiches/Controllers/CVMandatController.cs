using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    [Route("api/Mandat")]
    public class CVMandatController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public MandatGraphRepository mandatGraphRepository;
        public ProjetGraphRepository projetGraphRepository;
        public ClientGraphRepository clientGraphRepository;
        public EmployeurGraphRepository employeurGraphRepository;
        public TechnologieGraphRepository technologieGraphRepository;
        public FonctionGraphRepository fonctionGraphRepository;
        public TacheGraphRepository tacheGraphRepository;
        public ConseillerGraphRepository conseillerGraphRepository;

        public CVMandatController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            mandatGraphRepository = new MandatGraphRepository();
            projetGraphRepository = new ProjetGraphRepository();
            clientGraphRepository = new ClientGraphRepository();
            employeurGraphRepository = new EmployeurGraphRepository();
            technologieGraphRepository = new TechnologieGraphRepository();
            fonctionGraphRepository = new FonctionGraphRepository();
            tacheGraphRepository = new TacheGraphRepository();
            conseillerGraphRepository = new ConseillerGraphRepository();
        }

        [Route("{utilisateurId}/Detail/{mandatId}")]
        [AllowAnonymous]
        public ActionResult Detail(string utilisateurId, string mandatId)
        {
            var mandatViewModel = new MandatViewModel();

            var mandat = mandatGraphRepository.GetOne(mandatId);
            var projet = mandat.Projet;
            var client = projet.Client;
            var societeDeConseil = projet.SocieteDeConseil;

            mandatViewModel.DebutMandat = mandat.DateDebut;
            mandatViewModel.Efforts = mandat.Efforts;
            mandatViewModel.FinMandat = mandat.DateFin;
            mandatViewModel.Fonction = mandat.Fonction?.Description;
            mandatViewModel.GraphId = mandat.GraphKey;
            mandatViewModel.GraphIdFonction = mandat.Fonction?.GraphKey;
            mandatViewModel.NumeroMandat = Convert.ToInt32(mandat.Numero);
            mandatViewModel.PorteeDesTravaux = mandat.Description;
            mandatViewModel.Taches = GetTaches(utilisateurId, mandat);
            mandatViewModel.TitreMandat = mandat.Titre;

            mandatViewModel.DebutProjet = mandat.Projet.DateDebut;
            mandatViewModel.CellulaireReference = projet?.CellulaireReference;
            mandatViewModel.ContexteProjet = projet?.Description;
            mandatViewModel.CourrielReference = projet?.CourrielReference;
            mandatViewModel.Envergure = projet.Envergure;
            mandatViewModel.FinProjet = projet.DateFin;
            mandatViewModel.GraphIdProjet = projet?.GraphKey;
            mandatViewModel.FonctionReference = projet?.FonctionReference;
            mandatViewModel.NomReference = projet?.NomReference;
            mandatViewModel.Technologies = GetTechnologies(utilisateurId, mandat);
            mandatViewModel.TelephoneReference = projet?.TelephoneReference;
            mandatViewModel.TitreProjet = projet?.Nom;

            mandatViewModel.GraphIdClient = client?.GraphKey;
            mandatViewModel.NomClient = client?.Nom;

            mandatViewModel.GraphIdSocieteDeConseil = societeDeConseil?.GraphKey;
            mandatViewModel.NomEntreprise = societeDeConseil?.Nom;

            var editions = new EditionObjectViewModelFactory<MandatViewModel>();
            mandatViewModel.editionObjecViewModels = editions.GetEditions(mandat, projet, client, societeDeConseil);



            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DateFormatString = "yyyy/MM/dd";
            jsonSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            return Json(mandatViewModel, jsonSettings);
        }

        private List<TacheViewModel> GetTaches(string utilisateurId, Mandat mandat)
        {
            var noeudsModifie = new List<GraphObject> { mandat };
            var taches = mandat.Taches.Cast<GraphObject>().ToList();

            var tachesViewModels = ViewModelFactory<Tache, TacheViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: taches,
                map: mapTache);
            return tachesViewModels.ConvertAll(x => (TacheViewModel)x);
        }

        private List<TechnologieViewModel> GetTechnologies(string utilisateurId, Mandat mandat)
        {
            var noeudsModifie = new List<GraphObject> { mandat };
            var technologies = mandat.Projet.Technologies.Cast<GraphObject>().ToList();

            var techsViewModels = ViewModelFactory<Technologie, TechnologieViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudsModifie,
                graphObjects: technologies,
                map: mapTechnologie);

            return techsViewModels.ConvertAll(x => (TechnologieViewModel)x);
        }
        private ViewModel mapTache(GraphObject tacheModel)
        {
            var tache = (Tache)tacheModel;
            return new TacheViewModel
            {
                Description = tache.Description,
                GraphId = tache.GraphKey,
            };
        }

        private ViewModel mapTechnologie(GraphObject technologieModel)
        {
            var tech = (Technologie)technologieModel;
            return new TechnologieViewModel
            {
                Description = tech.Nom,
                GraphId = tech.GraphKey,
            };
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]MandatViewModel mandat)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var conseiller = utilisateur.Conseiller;

            var societeDeConseil = new Employeur { Nom = mandat.NomEntreprise };

            var client = new Client { Nom = mandat.NomClient };

            var projet = new Projet
            {
                DateDebut = mandat.DebutProjet,
                CellulaireReference = mandat.CellulaireReference,
                Description = mandat.ContexteProjet,
                CourrielReference = mandat.CourrielReference,
                Envergure = mandat.Envergure,
                DateFin = mandat.FinProjet,
                FonctionReference = mandat.FonctionReference,
                NomReference = mandat.NomReference,
                TelephoneReference = mandat.TelephoneReference,
                Nom = mandat.TitreProjet,
                SocieteDeConseil = societeDeConseil,
                Client = client,
                Technologies = mandat.Technologies.Select(x => technologieGraphRepository.GetOne(x.GraphId)).ToList()
            };

            var fonction = fonctionGraphRepository.GetOne(mandat.GraphIdFonction);

            var mandatModel = new Mandat
            {
                DateDebut = mandat.DebutMandat,
                Efforts = mandat.Efforts,
                DateFin = mandat.FinMandat,
                Numero = mandat.NumeroMandat.ToString(),
                Description = mandat.PorteeDesTravaux,
                Titre = mandat.TitreMandat,
                Fonction = fonction,
                Projet = projet,
                Taches = mandat.Taches.Select(x => tacheGraphRepository.Search(new Tache { Description = x.Description }, true).First()).ToList()
            };

            conseiller.Mandats.Add(mandatModel);
            mandatGraphRepository.Add(mandatModel);

            editionObjectGraphRepository.AjouterNoeud(objetAjoute: mandatModel, viewModelProprieteNom: "Mandats", noeudModifie: utilisateur.Conseiller);

            mandat.GraphId = mandatModel.GraphKey;


            return Json(mandat);
        }


        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]MandatViewModel mandat)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandatModel = mandatGraphRepository.GetOne(mandat.GraphId);
            var projet = mandatModel.Projet;
            var client = projet.Client;
            var societeDeConseil = projet.SocieteDeConseil;

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.DebutMandat,
                graphModelPropriete: () => mandatModel.DateDebut,
                noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.Efforts,
                graphModelPropriete: () => mandatModel.Efforts,
                noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.FinMandat,
                graphModelPropriete: () => mandatModel.DateFin,
                noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.NumeroMandat,
                graphModelPropriete: () => mandatModel.Numero,
                noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.PorteeDesTravaux,
                graphModelPropriete: () => mandatModel.Description,
                noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.TitreMandat,
                graphModelPropriete: () => mandatModel.Titre,
                noeudModifie: mandatModel);


            editionObjectGraphRepository.ChangerPropriete( 
                viewModelPropriete: () => mandat.GraphIdFonction,
                graphModelPropriete: () => mandatModel.Fonction.GraphKey,
                graphModelProprieteNom: "Fonction", noeudModifie: mandatModel);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.CellulaireReference,
                graphModelPropriete: () => projet.CellulaireReference,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.ContexteProjet,
                graphModelPropriete: () => projet.Description,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.ContexteProjet,
                graphModelPropriete: () => projet.Description,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.Envergure,
                graphModelPropriete: () => projet.Envergure,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.FinProjet,
                graphModelPropriete: () => projet.DateFin,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.FonctionReference,
                graphModelPropriete: () => projet.FonctionReference,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.NomReference,
                graphModelPropriete: () => projet.NomReference,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.TelephoneReference,
                graphModelPropriete: () => projet.TelephoneReference,
                noeudModifie: projet);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.TitreProjet,
                graphModelPropriete: () => projet.Nom,
                noeudModifie: projet);


            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.NomEntreprise,
                graphModelPropriete: () => projet.SocieteDeConseil.Nom,
                noeudModifie: societeDeConseil);

            editionObjectGraphRepository.ChangerPropriete(
                viewModelPropriete: () => mandat.NomClient,
                graphModelPropriete: () => projet.Client.Nom,
                noeudModifie: client);

            return Json(mandat);
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{mandatId}")]
        public ActionResult Delete(string utilisateurId, string mandatId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var mandat = mandatGraphRepository.GetOne(mandatId);

            var mandats = utilisateur.Conseiller.Mandats;

            if (mandats.Any(x => x.GraphKey == mandat.GraphKey))
            {
                foreach (var edition in mandat.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(mandat, "Mandats", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == mandat.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }



            return Json(mandat);
        }
    }
}