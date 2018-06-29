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
        public List<FormationScolaire> FormationsScolaires { get => (List<FormationScolaire>)LoadProperty("FormationsScolaires"); set => SetProperty("FormationsScolaires", value); }

        [Edge("IsMemberOf")]
        public List<OrdreProfessional> Associations { get => (List<OrdreProfessional>)LoadProperty("Associations"); set => SetProperty("Associations", value); }

        [Edge("Obtained")]
        public List<Formation> Formations { get => (List<Formation>)LoadProperty("Formations"); set => SetProperty("Formations", value); }

        [Edge("IsPartOf")]
        public Secteur Secteur { get => (Secteur)LoadProperty("Secteur"); set => SetProperty("Secteur", value); }

        [Edge("OfType")]
        public Genre Type { get => (Genre)LoadProperty("Type"); set => SetProperty("Type", value); }

        [Edge("Currently")]
        public Status Status { get => (Status)LoadProperty("Status"); set => SetProperty("Status", value); }

        [Edge("WorkAs")]
        public Fonction Fonction { get => (Fonction)LoadProperty("Fonction"); set => SetProperty("Fonction", value); }

        [Edge("WorkedIn")]
        public List<Mandat> Mandats { get => (List<Mandat>)LoadProperty("Mandats"); set => SetProperty("Mandats", value); }

        [Edge("Knows")]
        public List<Langue> Langues { get => (List<Langue>)LoadProperty("Langues"); set => SetProperty("Langues", value); }

        [Edge("Has")]
        public List<DomaineDIntervention> DomaineDInterventions { get => (List<DomaineDIntervention>)LoadProperty("DomaineDInterventions"); set => SetProperty("DomaineDInterventions", value); }

        [Edge("Has", true)]
        public List<CV> CVs { get => (List<CV>)LoadProperty("CVs"); set => SetProperty("CVs", value); }

        [Edge("PointTo")]
        public List<PieceJointe> PiecesJointes { get => (List<PieceJointe>)LoadProperty("PiecesJointes"); set => SetProperty("PiecesJointes", value); }

        [Edge("Knows", lazyLoad:true)]
        public List<Technologie> Technologies { get => (List<Technologie>)LoadProperty("Technologies"); set => SetProperty("Technologies", value); }

        [Edge("WorkedFor")]
        public List<Employeur> Employeurs { get => (List<Employeur>)LoadProperty("Employeurs"); set => SetProperty("Employeurs", value); }

        //[Edge("Was")]
        //public Conseiller VersionPrecedent { get => (Conseiller)LoadProperty("VersionPrecedent"); set => SetProperty("VersionPrecedent", value); }

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
