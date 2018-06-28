﻿using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Utilisateur : GraphObject
    {
        public int NumeroEmploye { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }

        public string AdresseCourriel { get; set; }

        [Edge("Is")]
        public Conseiller Conseiller { get; set; }
    }
}
