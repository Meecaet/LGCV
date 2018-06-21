using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Projet : GraphObject
    {
        [Edge("Advised", true)]
        public Employeur SocieteDeConseil { get; set; }
        [Edge("RequestedBy")]
        public Client Client { get; set; }

        public string Nom { get; set; }
        public int Envergure { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Telephonereference { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public double HeuresParSemaine { get; set; }

        [Edge("Used", true)]
        public List<Technologie> Technologies { get; set; }

        public Projet()
        {
            Technologies = new List<Technologie>();
        }
    }
}
