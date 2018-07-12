using DAL_CV_Fiches.Repositories.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class EditionObject : GraphObject
    {
        public string ObjetAjouteId { get; set; }
        public string ObjetSupprimeId { get; set; }
        public string Status { get; set; }
        public string Observacao { get; set; }
        public string Type { get; set; }

        [Edge("Modifier")]
        public GraphObject NoeudModifie { get; set; }

        [Edge("ValeurModifie")]
        public List<ProprieteModifiee> ProprietesModifiees { get; set; }

        public EditionObject()
        {
            ProprietesModifiees = new List<ProprieteModifiee>();
        }

        public static EditionObject CreateChangementRelation(string objetAjouteId, string objetSupprimeId, GraphObject noeudModifie)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ObjetAjouteId = objetAjouteId;
            edition.ObjetSupprimeId = objetSupprimeId;
            edition.Type = "ChangementRelation";

            CreateRelation(noeudModifie, edition);

            return edition;
        }

        public static EditionObject CreateAddition(string objetAjouteId, GraphObject noeudModifie)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ObjetAjouteId = objetAjouteId;
            edition.Type = "Addition";

            CreateRelation(noeudModifie, edition);

            return edition;
        }

        public static EditionObject CreateChangementPropriete(List<KeyValuePair<string, string>> proprietes, GraphObject noeudModifie)
        {
            var proprietesModifiees = proprietes.Select(x => new ProprieteModifiee() { Nom = x.Key, valeur = x.Value });

            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.Type = "ChangementPropriete";
            edition.ProprietesModifiees.AddRange(proprietesModifiees);

            CreateRelation(noeudModifie, edition);

            return edition;
        }

        public static EditionObject CreateEnlevement(string objetSupprimeId, GraphObject noeudModifie)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ObjetSupprimeId = objetSupprimeId;
            edition.Type = "Enlevement";

            CreateRelation(noeudModifie, edition);

            return edition;
        }


        private static void CreateRelation(GraphObject noeudModifie, EditionObject edition)
        {
            var editionRepository = new EditionObjectGraphRepository();
            editionRepository.Add(edition);

            noeudModifie.EditionObjects.Add(edition);
            var noeuModifieRepository = GraphRepositoy.GetGenericRepository(noeudModifie.GetType());

            var att = (Edge)noeudModifie.GetType().GetProperty("EditionObjects").GetCustomAttribute(typeof(Edge));
            noeuModifieRepository.GetType().GetMethod("CreateEdge").Invoke(noeuModifieRepository, new object[] { noeudModifie, edition, att });
        }
    }
}
