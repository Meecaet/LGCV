using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class EditionObjectGraphRepository : GraphRepositoy<EditionObject>
    {
        public EditionObjectGraphRepository()
        {
        }

        public EditionObjectGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public EditionObjectGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public EditionObjectGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }

        public EditionObject CreateOrUpdateChangementPropietes(List<KeyValuePair<string, string>> proprietes, GraphObject noeudModifie)
        {

            var edition = FindEditionObjectOfType(noeudModifie, EditionObjectType.ChangementPropriete, EditionObjectEtat.Modifie);
            if (edition != null)
            {
                var proprieteModifieeGraphRepository = new ProprieteModifieeGraphRepository();
                foreach (ProprieteModifiee prop in edition.ProprietesModifiees)
                {
                    var newProp = proprietes.Find(x => x.Key == prop.Nom);
                    prop.valeur = newProp.Value;
                    proprieteModifieeGraphRepository.Update(prop);
                }
            }
            else
            {
                var proprietesModifiees = proprietes.Select(x => new ProprieteModifiee() { Nom = x.Key, valeur = x.Value });

                edition = new EditionObject();
                edition.NoeudModifie = noeudModifie;
                edition.Type = EditionObjectType.ChangementPropriete;
                edition.Etat = EditionObjectEtat.Modifie;
                edition.ProprietesModifiees.AddRange(proprietesModifiees);

                CreateRelation(noeudModifie, edition);
            }


            return edition;

        }

        public EditionObject CreateOrUpdateChangementRelation(string objetAjouteId, string objetSupprimeId, GraphObject noeudModifie)
        {
            var edition = FindEditionObjectOfType(noeudModifie, EditionObjectType.ChangementRelation, EditionObjectEtat.Modifie);
            if (edition != null)
            {
                edition.ObjetAjouteId = objetAjouteId;
                edition.ObjetSupprimeId = objetSupprimeId;
                Update(edition);
            }
            else
            {
                edition = new EditionObject();
                edition.NoeudModifie = noeudModifie;
                edition.ObjetAjouteId = objetAjouteId;
                edition.ObjetSupprimeId = objetSupprimeId;
                edition.Etat = EditionObjectEtat.Modifie;
                edition.Type = EditionObjectType.ChangementRelation;

                CreateRelation(noeudModifie, edition);

            }

            return edition;
        }

        public EditionObject CreateAddition(string objetAjouteId, GraphObject noeudModifie)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ObjetAjouteId = objetAjouteId;
            edition.Etat = EditionObjectEtat.Modifie;
            edition.Type = EditionObjectType.Addition;

            CreateRelation(noeudModifie, edition);

            return edition;
        }


        public EditionObject CreateEnlevement(string objetSupprimeId, GraphObject noeudModifie)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ObjetSupprimeId = objetSupprimeId;
            edition.Etat = EditionObjectEtat.Modifie;
            edition.Type = "Enlevement";

            CreateRelation(noeudModifie, edition);

            return edition;
        }

        public EditionObject FindEditionObjectOfType(GraphObject graphnoeud, string type, string editionEtat)
        {
            return graphnoeud.EditionObjects.Find(x => x.Type == type && x.Etat == editionEtat);
        }

        private void CreateRelation(GraphObject noeudModifie, EditionObject edition)
        {
            Add(edition);

            noeudModifie.EditionObjects.Add(edition);
            var noeuModifieRepository = GraphRepositoy.GetGenericRepository(noeudModifie.GetType());
            var att = (Edge)noeudModifie.GetType().GetProperty("EditionObjects").GetCustomAttribute(typeof(Edge));
            noeuModifieRepository.GetType().GetMethod("CreateEdge").Invoke(noeuModifieRepository, new object[] { noeudModifie, edition, att });
        }

    }
}
