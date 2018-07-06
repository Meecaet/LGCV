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
            Parle = Ecrit = Lu = Niveau.avancé;
        }
    }

    public enum Niveau
    {
<<<<<<< HEAD
        basique = 0,
        débutant = 0,
        intermédiaire = 1,
        avancé = 2
=======
        Basique = 0,
        Débutant = 0,
        Intermédiaire = 1,
        Baseintermédiaire,
        Avancé = 2,
        Bon = 2
>>>>>>> 67c0222d48f658fb85a9400f927a853e898d7797
    }
}
