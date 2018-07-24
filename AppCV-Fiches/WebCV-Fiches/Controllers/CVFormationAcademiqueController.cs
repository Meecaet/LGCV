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
    [Route("FormationAcademique")]
    public class CVFormationAcademiqueController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public FormationScolaireGraphRepository formationScolaireGraphRepository;
        public InstituitionGraphRepository instituitionGraphRepository;

        public CVFormationAcademiqueController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            formationScolaireGraphRepository = new FormationScolaireGraphRepository();
            instituitionGraphRepository = new InstituitionGraphRepository();
        }

        [Route("{utilisateurId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {
            Func<GraphObject, ViewModel> map = this.map;

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var formations = utilisateur.Conseiller.FormationsScolaires.Cast<GraphObject>().ToList(); ;
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(formations);
            var formationsViewModel = ViewModelFactory.GetViewModels(utilisateurId: utilisateurId, noeudsModifie: noeudModifie, graphObjects: formations, map: map);
            return Json(formationsViewModel);
        }

        private ViewModel map(GraphObject formationScolaireModel)
        {
            var formation = (FormationScolaire)formationScolaireModel;
            return new FormationAcademiqueViewModel
            {
                Annee = formation.DateConclusion,
                Diplome = formation.Diplome,
                GraphIdEtablissement = formation.Ecole?.GraphKey,
                GraphId = formation.GraphKey,
                Etablissement = formation.Ecole?.Nom,
                Niveau = formation.Niveau.ToString(),
                Pays = formation.Ecole?.Pays?.Nom,
                Principal = formation.EstPrincipal
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]FormationAcademiqueViewModel formationAcademique)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var instituition = new Instituition() { Nom = formationAcademique.Etablissement, Pays = new Pays { Nom = formationAcademique.Pays } };

            instituition = instituitionGraphRepository.Search(instituition, true).First();

            var formationAcademiqueModel = FormationScolaire.CreateFormationScolaire(
                diplome: formationAcademique.Diplome,
                dateConlusion: formationAcademique.Annee,
                equivalence: false,
                niveau: formationAcademique.Niveau,
                principal: formationAcademique.Principal,
                instituition: instituition
                );

            formationScolaireGraphRepository.Add(formationAcademiqueModel);
            editionObjectGraphRepository.AjouterNoeud(objetAjoute: formationAcademiqueModel, noeudModifiePropriete: "FormationsScolaires", noeudModifie: utilisateur.Conseiller);

            formationAcademique.GraphId = formationAcademiqueModel.GraphKey;
            return Json(formationAcademique);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]FormationAcademiqueViewModel formationAcademique)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status="OK", Message="Formation académique modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{formationAcademiqueId}")]
        public ActionResult Delete(string utilisateurId, string formationAcademiqueId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Formation académique eliminé" });
        }
    }
}