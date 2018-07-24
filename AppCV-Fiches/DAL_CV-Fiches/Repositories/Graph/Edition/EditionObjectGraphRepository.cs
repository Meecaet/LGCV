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
        public EditionObjectGraphRepository() { }

        public EditionObjectGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection) { }

        public EditionObjectGraphRepository(string Database, string Graph) : base(Database, Graph) { }

        public EditionObjectGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph) { }

        public List<EditionObject> CreateOrUpdateProprieteEdition(List<KeyValuePair<string, string>> proprietes, GraphObject noeudModifie)
        {
            List<EditionObject> editions = new List<EditionObject>();
            EditionObject edition = new EditionObject();

            foreach (KeyValuePair<string, string> prop in proprietes)
            {
                edition = FindEditionObjectOfType(proprieteNom: prop.Key, graphnoeud: noeudModifie, type: EditionObjectType.ChangementPropriete, editionEtat: EditionObjectEtat.Modifie);
                if (edition != null)
                {
                    edition.ProprieteValeur = prop.Value;
                    Update(edition);
                }
                else
                {
                    edition = CreateEditionObject(proprieteNom: prop.Key, proprieteValuer: prop.Value, noeudModifie: noeudModifie);
                }
                editions.Add(edition);
            }

            return editions;
        }

        public EditionObject ChangerNoeud(string objetAjouteGraphKey, string objetsupprimeGraphKey, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            var edition = FindEditionObjectOfType(
                proprieteNom: noeudModifiePropriete,
                graphnoeud: noeudModifie,
                type: EditionObjectType.ChangementRelation,
                editionEtat: EditionObjectEtat.Modifie);

            if (edition != null)
            {
                edition.ObjetAjouteId = objetAjouteGraphKey;
                edition.ObjetSupprimeId = objetsupprimeGraphKey;
                Update(edition);
            }
            else
            {
                edition = CreateEditionObject(
                    proprieteNom: noeudModifiePropriete,
                    objetAjouteId: objetAjouteGraphKey,
                    objetSupprimeId: objetsupprimeGraphKey,
                    noeudModifie: noeudModifie);
            };
            return edition;
        }

        public EditionObject AjouterNoeud(GraphObject objetAjoute, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            return CreateEditionObject(proprieteNom: noeudModifiePropriete, objetAjouteId: objetAjoute.GraphKey, noeudModifie: noeudModifie);
        }

        public EditionObject SupprimerNoeud(GraphObject objetsupprime, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            return CreateEditionObject(proprieteNom: noeudModifiePropriete, objetSupprimeId: objetsupprime.GraphKey, noeudModifie: noeudModifie);
        }

        private EditionObject CreateEditionObject(string proprieteNom, GraphObject noeudModifie, string proprieteValuer = null, string objetAjouteId = null, string objetSupprimeId = null)
        {
            var edition = new EditionObject();
            edition.NoeudModifie = noeudModifie;
            edition.ProprieteNom = proprieteNom;
            if (proprieteValuer != null)
            {
                edition.Type = EditionObjectType.ChangementPropriete;
                edition.ProprieteValeur = proprieteValuer;
            }
            else
            {
                edition.Type = EditionObjectType.ChangementRelation;
                if (objetSupprimeId != null)
                    edition.ObjetSupprimeId = objetSupprimeId;

                if (objetAjouteId != null)
                    edition.ObjetAjouteId = objetAjouteId;
            }

            edition.Etat = EditionObjectEtat.Modifie;

            CreateRelation(noeudModifie, edition);

            return edition;
        }

        private EditionObject FindEditionObjectOfType(string proprieteNom, GraphObject graphnoeud, string type, string editionEtat)
        {
            return graphnoeud.EditionObjects.Find(x => x.ProprieteNom == proprieteNom && x.Type == type && x.Etat == editionEtat);
        }

        private void CreateRelation(GraphObject noeudModifie, EditionObject edition)
        {
            Add(edition, false);
            var att = (Edge)edition.GetType().GetProperty("NoeudModifie").GetCustomAttribute(typeof(Edge));
            CreateEdge(edition, noeudModifie, att);

            noeudModifie.EditionObjects.Add(edition);
            dynamic noeuModifieRepository = GraphRepositoy.GetGenericRepository(noeudModifie.GetType());
            att = (Edge)noeudModifie.GetType().GetProperty("EditionObjects").GetCustomAttribute(typeof(Edge));
            noeuModifieRepository.CreateEdge(noeudModifie, edition, att);
        }

    }
}
