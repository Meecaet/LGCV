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
        [Edge("Ajoute", lazyLoad: true)]
        public GraphObject ObjetAjoute { get => (GraphObject)LoadProperty("ObjetAjoute"); set => SetProperty("ObjetAjoute", value); }
        public string ObjetSupprimeId { get; set; }
        public string Etat { get; set; }
        public string Observacao { get; set; }
        public string Type { get; set; }
        public string GraphModelProprieteNom { get; set; } 
        public string ViewModelProprieteNom { get; set; } 
        public string ProprieteValeur { get; set; } 

        [Edge("Modifier", lazyLoad: true)]
        public GraphObject NoeudModifie { get => (GraphObject)LoadProperty("NoeudModifie"); set => SetProperty("NoeudModifie", value); }
    }


    public static class EditionObjectType
    {
        public static string ChangementPropriete = "ChangementPropriete";
        public static string ChangementRelation = "ChangementRelation";
    }

    public static class EditionObjectEtat
    {
        public static string Modifie = "Modifie";
        public static string PourApprouve = "PourApprouve";
    }
}
