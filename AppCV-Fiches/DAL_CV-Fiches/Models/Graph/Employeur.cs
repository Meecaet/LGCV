using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Employeur : GraphObject
    {
        public string Nom { get; set; }
        public string Periode { get; set; }
        public string DescriptionDuTravail { get; set; }

        [Edge("Consults")]
        public List<Client> Clients { get; set; }

        public Employeur()
        {
            Clients = new List<Client>();
        }        

    }
}
