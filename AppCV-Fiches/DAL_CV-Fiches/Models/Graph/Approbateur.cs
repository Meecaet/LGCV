using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Approbateur : GraphObject
    {
        [Edge("Est")]
        public Utilisateur Utilisateur { get; set; }
        [Edge("Approuve")]
        public List<CV> CVsApprouves { get; set; }
    }
}
