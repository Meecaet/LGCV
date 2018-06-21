using System.Linq;
using System.Collections.Generic;
using System;
using DAL_CV_Fiches.Repositories.Graph.Attributes;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Langue : GraphObject
    {
        public string Nom { get; set; }

        [EdgeProperty]
        public Niveau Parle { get; set; }
        [EdgeProperty]
        public Niveau Ecrit { get; set; }
        [EdgeProperty]
        public Niveau Lu { get; set; }

        public Langue()
        {
            Parle = Ecrit = Lu = Niveau.Avancé;
        }
    }

    public enum Niveau
    {
        Basique = 0,
        Intermédiaire = 1,
        Avancé = 2
    }
}