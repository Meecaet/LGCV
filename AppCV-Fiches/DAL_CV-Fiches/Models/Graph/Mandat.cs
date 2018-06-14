using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Mandat : GraphObject
    {
        public string Numero { get; set; }
        public string Projet { get; set; }
        public string Envenrgure { get; set; }
        public string Fonction { get; set; }
        public string Periode { get; set; }
        public string Efforts { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

        [Repositories.Graph.Attributes.Edge("Used")]
        public List<Technologie> Technologies { get; set; }

        public Mandat()
        {
            Technologies = new List<Technologie>();
        }
    }
}
