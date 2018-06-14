using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Perfectionnement : GraphObject
    {
        [EdgeProperty]
        public string An { get; set; }
        public string Description { get; set; }       

    }
}
