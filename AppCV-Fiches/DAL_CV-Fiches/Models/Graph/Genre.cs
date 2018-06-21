using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Genre : GraphObject
    {
        public string Description { get; set; }
        public string Descriminator { get; set; }
    }
}
