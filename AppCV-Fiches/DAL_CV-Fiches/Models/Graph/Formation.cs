using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Formation : GraphObject
    {
        public string Description { get; set; }
        public int AnAcquisition { get; set; }

        [Edge("DuType")]
        public Genre Type { get; set; }
    }
}
