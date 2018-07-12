using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class ChangementRelation : EditionObject
    {
        public string ObjetAjouteId { get; set; }
        public string ObjetSupprimeId { get; set; }

        public static ChangementRelation Create(string objetAjouteId, string objetSupprimeId, GraphObject noeudModifie)
        {
            var changement = new ChangementRelation();
            changement.NoeudModifie = noeudModifie;
            changement.ObjetAjouteId = objetAjouteId;
            changement.ObjetSupprimeId = objetSupprimeId;
            return changement;
        }
    }
}
