using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
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

        public EditionObject ChangerNoeud(GraphObject objetAjoute, string objetsupprimeGraphKey, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            var edition = FindEditionObjectOfType(
                proprieteNom: noeudModifiePropriete,
                graphnoeud: noeudModifie,
                type: EditionObjectType.ChangementRelation,
                editionEtat: EditionObjectEtat.Modifie);

            if (edition != null)
            {
                edition.ObjetAjoute = objetAjoute;
                edition.ObjetSupprimeId = objetsupprimeGraphKey;
                Update(edition);
                if (edition.ObjetAjoute != null)
                {
                    string deleteAjouteEdgeQuery = $"g.V('{edition.GraphKey}').outE('Ajoute').drop()";
                    ExecuteCommandQueryVertex(deleteAjouteEdgeQuery);
                    var att = (Edge)edition.GetType().GetProperty("ObjetAjoute").GetCustomAttribute(typeof(Edge));
                    CreateEdge(edition, edition.ObjetAjoute, att);
                }
            }
            else
            {
                edition = CreateEditionObject(
                    proprieteNom: noeudModifiePropriete,
                    objetAjoute: objetAjoute,
                    objetSupprimeId: objetsupprimeGraphKey,
                    noeudModifie: noeudModifie);
            };
            return edition;
        }

        public EditionObject AjouterNoeud(GraphObject objetAjoute, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            return CreateEditionObject(proprieteNom: noeudModifiePropriete, objetAjoute: objetAjoute, noeudModifie: noeudModifie);
        }

        public EditionObject SupprimerNoeud(GraphObject objetsupprime, string noeudModifiePropriete, GraphObject noeudModifie)
        {
            return CreateEditionObject(proprieteNom: noeudModifiePropriete, objetSupprimeId: objetsupprime.GraphKey, noeudModifie: noeudModifie);
        }

        public override void Delete(EditionObject obj)
        {
            string deleteAjouteEdgeQuery = $"g.V('{obj.GraphKey}').outE('Ajoute').drop()";
            ExecuteCommandQueryVertex(deleteAjouteEdgeQuery);

            string deleteModifierEdgeQuery = $"g.V('{obj.GraphKey}').outE('Modifier').drop()";
            ExecuteCommandQueryVertex(deleteModifierEdgeQuery);

            string deleteEteModifieQuery = $"g.V('{obj.GraphKey}').inE('EteModifie').drop()";
            ExecuteCommandQueryVertex(deleteEteModifieQuery);

            base.Delete(obj);
        }




        private EditionObject CreateEditionObject(string proprieteNom, GraphObject noeudModifie, string proprieteValuer = null, GraphObject objetAjoute = null, string objetSupprimeId = null)
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

                if (objetAjoute != null)
                    edition.ObjetAjoute = objetAjoute;
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

            if (edition.ObjetAjoute != null)
            {
                att = (Edge)edition.GetType().GetProperty("ObjetAjoute").GetCustomAttribute(typeof(Edge));
                CreateEdge(edition, edition.ObjetAjoute, att);
            }

            noeudModifie.EditionObjects.Add(edition);
            dynamic noeuModifieRepository = GraphRepositoy.GetGenericRepository(noeudModifie.GetType());
            att = (Edge)noeudModifie.GetType().GetProperty("EditionObjects").GetCustomAttribute(typeof(Edge));
            noeuModifieRepository.CreateEdge(noeudModifie, edition, att);
        }

    }
}
