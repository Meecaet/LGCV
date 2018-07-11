using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class EditionObject : GraphObject
    {
        [Edge("Modifier")]
        public GraphObject NoeudModifie { get; set; }
    }
}
