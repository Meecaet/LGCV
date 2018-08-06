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
    [Route("api/Langue")]
    public class CVLangueController : Controller
    {
        public EditionObjectGraphRepository editionObjectGraphRepository;
        public UtilisateurGraphRepository utilisateurGraphRepository;
        public LangueGraphRepository langueGraphRepository;

        public CVLangueController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
            editionObjectGraphRepository = new EditionObjectGraphRepository();
            langueGraphRepository = new LangueGraphRepository();
        }

        [Route("{utilisateurId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {
            Func<GraphObject, ViewModel> map = this.map;

            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var langues = utilisateur.Conseiller?.Langues?.Cast<GraphObject>().ToList();
            if(langues == null)    
                return Json(new List<LangueViewModel>());
            var noeudModifie = new List<GraphObject>();
            noeudModifie.Add(utilisateur.Conseiller);
            noeudModifie.AddRange(langues);
            var languesViewModel = ViewModelFactory<Langue, LangueViewModel>.GetViewModels(
                utilisateurId: utilisateurId,
                noeudsModifie: noeudModifie,
                graphObjects: langues,
                map: map);
            return Json(languesViewModel);
        }

        private ViewModel map(GraphObject langueModel)
        {
            var langue = (Langue)langueModel;
            return new LangueViewModel
            {
                GraphId = langue.GraphKey,
                Nom = langue.Nom,
                NiveauEcrit = langue.Ecrit.ToString(),
                NiveauParle = langue.Parle.ToString(),
                NiveauLu = langue.Lu.ToString()
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Add")]
        public ActionResult Add(string utilisateurId, [FromBody]LangueViewModel langue)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            var langueModel = langueGraphRepository.CreateIfNotExists(new Langue { Nom = langue.Nom });
            langueModel.Parle = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauParle);
            langueModel.Ecrit = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauEcrit);
            langueModel.Lu = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauLu);

            editionObjectGraphRepository.AjouterNoeud(
                objetAjoute: langueModel,
                noeudModifiePropriete: "Langues",
                noeudModifie: utilisateur.Conseiller);

            langue.GraphId = langueModel.GraphKey;
            return Json(langue);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]LangueViewModel langue)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var langueModel = langueGraphRepository.GetOne(langue.GraphId);

            var langues = utilisateur.Conseiller.Langues;

            if (langues.Any(x => x.GraphKey == langue.GraphId))
            {

                var proprietesModifiees = new List<KeyValuePair<string, string>>();

                if (langueModel.Lu.ToString() != langue.NiveauLu)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("NiveauLu", langue.NiveauLu));

                if (langueModel.Parle.ToString() != langue.NiveauParle)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("NiveauParle", langue.NiveauParle));

                if (langueModel.Ecrit.ToString() != langue.NiveauEcrit)
                    proprietesModifiees.Add(new KeyValuePair<string, string>("NiveauEcrit", langue.NiveauEcrit));

                if (proprietesModifiees.Count() > 0)
                    editionObjectGraphRepository.CreateOrUpdateProprieteEdition(proprietesModifiees, langueModel);
            }
            else
            {
                langueModel.Parle = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauParle);
                langueModel.Ecrit = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauEcrit);
                langueModel.Lu = (Niveau)Enum.Parse(typeof(Niveau), langue.NiveauLu);
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute?.GraphKey == langue.GraphId);
                langueGraphRepository.Update(langueModel, edition);
            }

            return Json(langue);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Delete/{langueId}")]
        public ActionResult Delete(string utilisateurId, string langueId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);
            var langue = langueGraphRepository.GetOne(langueId);


            var langues = utilisateur.Conseiller.Langues;

            if (langues.Any(x => x.GraphKey == langue.GraphKey))
            {
                foreach (var edition in langue.EditionObjects)
                {
                    editionObjectGraphRepository.Delete(edition);
                }
                editionObjectGraphRepository.SupprimerNoeud(langue, "Langues", utilisateur.Conseiller);
            }
            else
            {
                var edition = utilisateur.Conseiller.EditionObjects.Find(x => x.ObjetAjoute.GraphKey == langue.GraphKey);
                editionObjectGraphRepository.Delete(edition);
            }



            return Json(langue);
        }
    }
}