using System.Linq;
using System.Collections.Generic;
using System;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Langue
    {
        public string Nom { get; set; }
        public Niveau Parle { get; set; }
        public Niveau Ecrit { get; set; }
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