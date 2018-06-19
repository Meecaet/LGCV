using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models
{
    public class Technologie : GraphObject
    {
        public string Nom { get; set; }
        public string Description { get; set; }

        [EdgeProperty]
        public int MoisDExperience { get; set; }
    }
}
