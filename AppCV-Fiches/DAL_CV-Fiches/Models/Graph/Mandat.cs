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
        [Edge("PourTravaillerDans")]
        public Projet Projet { get => (Projet)LoadProperty("Projet"); set => SetProperty("Projet", value); }

        public string Numero { get; set; }

        [Edge("PourTravailleComme")]
        public Fonction Fonction { get => (Fonction)LoadProperty("Fonction"); set => SetProperty("Fonction", value); }

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int Efforts { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        [Edge("PourRealiser")]
        public List<Tache> Taches { get => (List<Tache>)LoadProperty("Taches"); set => SetProperty("Taches", value); }

        public Mandat()
        {
            //Taches = new List<Tache>();
        }

    }
}
