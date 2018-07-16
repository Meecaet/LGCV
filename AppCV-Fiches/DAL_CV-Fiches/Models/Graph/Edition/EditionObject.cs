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
        public string Etat { get; set; }
        public string Observacao { get; set; }
        public string Type { get; set; }

        [Edge("Modifier")]
        public GraphObject NoeudModifie { get => (GraphObject)LoadProperty("NoeudModifie"); set => SetProperty("NoeudModifie", value); }

        [Edge("ValeurModifie")]
        public List<ProprieteModifiee> ProprietesModifiees { get; set; }

        public EditionObject()
        {
            ProprietesModifiees = new List<ProprieteModifiee>();
        }
    }


    public static class EditionObjectType
    {
        public static string Addition = "Addition";
        public static string ChangementPropriete = "ChangementPropriete";
        public static string Enlevement = "Enlevement";
        public static string ChangementRelation = "ChangementRelation";
    }

    public static class EditionObjectEtat
    {
        public static string Modifie = "Modifie";
        public static string PourApprouve = "PourApprouve";
    }
}
