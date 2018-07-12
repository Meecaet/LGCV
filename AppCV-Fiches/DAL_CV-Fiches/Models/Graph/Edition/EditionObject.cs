using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class EditionObject : GraphObject
    {
        public string Status { get; set; }
        public string Observacao { get; set; }

        [Edge("Modifier")]
        public GraphObject NoeudModifie { get; set; }
    }
}
