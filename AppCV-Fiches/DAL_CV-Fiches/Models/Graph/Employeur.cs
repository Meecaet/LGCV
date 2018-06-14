using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Employeur
    {
        public string Nom { get; set; }
        public string Periode { get; set; }
        public string DescriptionDuTravail { get; set; }
        public List<Client> Clients { get; set; }

        public Employeur()
        {
            Clients = new List<Client>();
        }        

    }
}
