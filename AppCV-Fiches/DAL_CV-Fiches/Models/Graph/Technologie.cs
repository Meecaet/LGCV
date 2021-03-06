﻿using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Technologie : GraphObject
    {
        public string Nom { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }

        [Edge("EstPartieDe", lazyLoad: false)]
        public CategorieDeTechnologie Categorie { get; set; }

        [EdgeProperty]
        public int MoisDExperience { get; set; }
    }
}
