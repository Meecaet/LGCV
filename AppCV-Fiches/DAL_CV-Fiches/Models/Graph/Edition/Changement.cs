using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Changement : EditionObject
    {
        [Edge("EteModifie")]
        public List<ProprieteModifiee> ProprietesModifiees { get; set; }

        public Changement()
        {
            ProprietesModifiees = new List<ProprieteModifiee>();
        }


        public static Changement Create(List<KeyValuePair<string, string>> proprietes, GraphObject noeudModifie)
        {
            var proprietesModifiees = proprietes.Select(x => new ProprieteModifiee() { Nom = x.Key, valeur = x.Value });
            var changement = new Changement();
            changement.NoeudModifie = noeudModifie;
            changement.ProprietesModifiees.AddRange(proprietesModifiees);
            return changement;
        }
    }
}
