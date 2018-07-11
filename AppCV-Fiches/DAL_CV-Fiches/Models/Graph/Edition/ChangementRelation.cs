using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class ChangementRelation : EditionObject
    {
        public string ObjetAjouteId { get; set; }
        public string ObjetSupprimeId { get; set; }
    }
}
