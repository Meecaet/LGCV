using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Mandat : GraphObject
    {
        [Edge("ToWorkIn")]
        public Projet Projet { get; set; }

        public string Numero { get; set; }   
        
        [Edge("WorkedAs")]
        public Fonction Fonction { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int Efforts { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        [Edge("Performed")]
        public List<Tache> Taches { get; set; }

    }
}
