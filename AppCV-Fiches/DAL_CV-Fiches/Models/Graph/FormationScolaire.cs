using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class FormationScolaire : GraphObject
    {
        public string Diplome { get; set; }
        [EdgeProperty]
        public DateTime DateConclusion { get; set; }
        public NiveauScolarite Niveau { get; set; }
        [EdgeProperty]
        public bool EstEquivalenceObtenu { get; set; }
        [EdgeProperty]
        public bool EstPrincipal { get; set; }

        [Edge("IssuedBy")]
        public Instituition Ecole { get; set; }
    }

    public enum NiveauScolarite
    {
        Primaire = 0,
        Secondaire = 1,
        DEC = 3,
        BAC = 4,
        Maitre = 5,
        Doctorat = 6,
        Nule = 9
    }
}
