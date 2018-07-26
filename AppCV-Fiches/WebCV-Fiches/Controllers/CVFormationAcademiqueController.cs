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
            formationAcademique.GraphIdEtablissement = formationAcademiqueModel.Ecole.GraphKey;
            return Json(formationAcademique);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]FormationAcademiqueViewModel formationAcademique)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var formationScolaireModel = formationScolaireGraphRepository.GetOne(formationAcademique.GraphId);

            var formationsAcademiques = utilisateur.Conseiller.FormationsScolaires;

            if (formationsAcademiques.Any(x => x.GraphKey == formationAcademique.GraphId))
            {

                var proprietesModifiees = new List<KeyValuePair<string, string>>();

                if (formationScolaireModel.DateConclusion != formationAcademique.Annee)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("Annee", formationAcademique.Annee.ToString()));

                if (formationScolaireModel.Diplome != formationAcademique.Diplome)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("Diplome", formationAcademique.Diplome));

                if (proprietesModifiees.Count() > 0)
                    editionObjectGraphRepository.CreateOrUpdateProprieteEdition(proprietesModifiees, formationScolaireModel);
            }
            else
            {
                formationScolaireModel.DateConclusion = formationAcademique.Annee;
                formationScolaireModel.Diplome = formationAcademique.Diplome;
                formationScolaireGraphRepository.Update(formationScolaireModel);
            }

            return Json(formationsAcademiques);
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{formationAcademiqueId}")]
        public ActionResult Delete(string utilisateurId, string formationAcademiqueId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var formation = formationScolaireGraphRepository.GetOne(formationAcademiqueId);


            var formations = utilisateur.Conseiller.FormationsScolaires;

            if (formations.Any(x => x.GraphKey == formation.GraphKey))
            {
                foreach (var edition in formation.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(formation, "FormationsScolaires", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute.GraphKey == formation.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }



            return Json(formation);
        }
    }
}