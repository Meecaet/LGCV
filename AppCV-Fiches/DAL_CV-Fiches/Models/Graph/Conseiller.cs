using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Edge("Etudie")]
        public List<FormationScolaire> FormationsScolaires { get => (List<FormationScolaire>)LoadProperty("FormationsScolaires"); set => SetProperty("FormationsScolaires", value); }

        [Edge("EstMembreDe")]
        public List<OrdreProfessional> Associations { get => (List<OrdreProfessional>)LoadProperty("Associations"); set => SetProperty("Associations", value); }

        [Edge("AObtenu")]
        public List<Formation> Formations { get => (List<Formation>)LoadProperty("Formations"); set => SetProperty("Formations", value); }

        [Edge("EstPartieDe")]
        public Secteur Secteur { get => (Secteur)LoadProperty("Secteur"); set => SetProperty("Secteur", value); }

        [Edge("DuType")]
        public Genre Type { get => (Genre)LoadProperty("Type"); set => SetProperty("Type", value); }

        [Edge("EstActuellement")]
        public Status Status { get => (Status)LoadProperty("Status"); set => SetProperty("Status", value); }

        [Edge("TravailleComme")]
        public Fonction Fonction { get => (Fonction)LoadProperty("Fonction"); set => SetProperty("Fonction", value); }

        [Edge("ATravailleDans", lazyLoad: true)]
        public List<Mandat> Mandats { get => (List<Mandat>)LoadProperty("Mandats"); set => SetProperty("Mandats", value); }

        [Edge("Connait")]
        public List<Langue> Langues { get => (List<Langue>)LoadProperty("Langues"); set => SetProperty("Langues", value); }

        [Edge("Appartient")]
        public List<DomaineDIntervention> DomaineDInterventions { get => (List<DomaineDIntervention>)LoadProperty("DomaineDInterventions"); set => SetProperty("DomaineDInterventions", value); }

        [Edge("Appartient", true)]
        public List<CV> CVs { get => (List<CV>)LoadProperty("CVs"); set => SetProperty("CVs", value); }

        [Edge("PointeVers")]
        public List<PieceJointe> PiecesJointes { get => (List<PieceJointe>)LoadProperty("PiecesJointes"); set => SetProperty("PiecesJointes", value); }

        [Edge("Connait")]
        public List<Technologie> Technologies { get => (List<Technologie>)LoadProperty("Technologies"); set => SetProperty("Technologies", value); }

        [Edge("ATravaillePour")]
        public List<Employeur> Employeurs { get => (List<Employeur>)LoadProperty("Employeurs"); set => SetProperty("Employeurs", value); }

        public Conseiller()
        {
        }

        public List<Formation> Certifications()
        {
            return Formations.Where(x =>
                        x.Type.Descriminator == Formation.Discriminator &&
                        x.Type.Description == FormationType.Certification
            ).ToList();
        }

        public List<Formation> Perfectionnement()
        {
            return Formations.Where(x =>
                        x.Type.Descriminator == Formation.Discriminator &&
                        x.Type.Description == FormationType.Perfectionnement
            ).ToList();
        }

        public List<Formation> Publication()
        {
            return Formations.Where(x =>
                        x.Type.Descriminator == Formation.Discriminator &&
                        x.Type.Description == FormationType.Publication
            ).ToList();
        }

        public List<Formation> Conference()
        {
            return Formations.Where(x =>
                        x.Type.Descriminator == Formation.Discriminator &&
                        x.Type.Description == FormationType.Conference
            ).ToList();
        }
    }
}
