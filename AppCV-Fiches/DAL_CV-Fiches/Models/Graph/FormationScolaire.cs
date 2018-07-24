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
        public int DateConclusion { get; set; }
        public NiveauScolarite Niveau { get; set; }
        [EdgeProperty]
        public bool EstEquivalenceObtenu { get; set; }
        [EdgeProperty]
        public bool EstPrincipal { get; set; }

        [Edge("DelivrePar")]
        public Instituition Ecole { get; set; }

        public static FormationScolaire CreateFormationScolaire(string diplome, int dateConlusion, string niveau, bool equivalence, bool principal, Instituition instituition)
        {
            return new FormationScolaire()
            {
                DateConclusion = dateConlusion,
                Diplome = diplome,
                EstEquivalenceObtenu = equivalence,
                EstPrincipal = principal,
                Niveau = (NiveauScolarite)Enum.Parse(typeof(NiveauScolarite), niveau),
                Ecole = instituition
            };
        }

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
