using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
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
    }
}
