﻿using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Projet : GraphObject
    {
        [Edge("EteConseillePar", true)]
        public Employeur SocieteDeConseil { get => (Employeur)LoadProperty("SocieteDeConseil"); set => SetProperty("SocieteDeConseil", value); }
        [Edge("DemandePar")]
        public Client Client { get => (Client)LoadProperty("Client"); set => SetProperty("Client", value); }

        public string Nom { get; set; }
        public int Envergure { get; set; }
        public string NomReference { get; set; }
        public string Description { get; set; }
        public string TelephoneReference { get; set; }
        public string CellulaireReference { get; set; }
        public string CourrielReference { get; set; }
        public string FonctionReference { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public double HeuresParSemaine { get; set; }

        [Edge("AUtilise", true)]
        public List<Technologie> Technologies { get => (List<Technologie>)LoadProperty("Technologies"); set => SetProperty("Technologies", value); }

        public Projet()
        {
            //Technologies = new List<Technologie>();
        }
    }
}
