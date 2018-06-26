using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Conseiller : GraphObject
    {
        public DateTime DateEmbauche { get; set; }
        public DateTime DateDepart { get; set; }
        public string RaisonDepart { get; set; }
        public string Logo { get; set; }

        [Edge("Studied")]
        public List<FormationScolaire> FormationsScolaires { get; set; }

        [Edge("IsMemberOf")]
        public List<OrdreProfessional> Associations { get; set; }

        [Edge("Obtained")]
        public List<Formation> Formations { get; set; }

        [Edge("IsPartOf")]
        public Secteur Secteur { get; set; }

        [Edge("OfType")]
        public Genre Type { get; set; }

        [Edge("Currently")]
        public Status Status { get; set; }        

        [Edge("WorkAs")]
        public Fonction Fonction { get; set; }

        [Edge("WorkedIn")]
        public List<Mandat> Mandats { get; set; }

        [Edge("Knows")]
        public List<Langue> Langues { get; set; }

        [Edge("Has")]
        public List<DomaineDIntervention> DomaineDInterventions { get; set; }

        [Edge("Has", true)]
        public List<CV> CVs { get; set; }

        [Edge("PointTo")]
        public List<PieceJointe> PiecesJointes { get; set; }

        [Edge("Knows")]
        public List<Technologie> Technologies { get; set; }

        [Edge("WorkedFor")]
        public List<Employeur> Employeurs { get; set; }

        [Edge("Was")]
        public Conseiller VersionPrecedent { get; set; }

        public Conseiller()
        {
            Employeurs = new List<Employeur>();
            Technologies = new List<Technologie>();
            PiecesJointes = new List<PieceJointe>();
            CVs = new List<CV>();
            Langues = new List<Langue>();
            Mandats = new List<Mandat>();
            Associations = new List<OrdreProfessional>();
            FormationsScolaires = new List<FormationScolaire>();
            Formations = new List<Formation>();
            DomaineDInterventions = new List<DomaineDIntervention>();
        }
    }
}
