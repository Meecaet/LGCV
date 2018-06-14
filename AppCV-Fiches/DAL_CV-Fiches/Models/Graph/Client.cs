using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Client : GraphObject
    {
        public string Nom { get; set; }

        [Edge("Has")]
        public List<Mandat> Mandats { get; set; }

        public Client()
        {
            Mandats = new List<Mandat>();
        }        
    }
}
