using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class CV : GraphObject
    {
        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        [Edge("Has")]
        public List<DomaineDIntervention> DomainesDIntervention { get; set; }

        [Edge("Graduated")]
        public List<FormationAcademique> FormationsAcademique { get; set; }

        [Edge("Certifieded")]
        public List<Certification> Certifications { get; set; }

        [Edge("WorkedFor")]
        public List<Employeur> Employeurs { get; set; }

        [Edge("Learned")]
        public List<Perfectionnement> Perfectionnements { get; set; }

        [Edge("Attended")]
        public List<Conference> Conferences { get; set; }

        [Edge("IsPartOf")]
        public List<Association> Associations { get; set; }

        [Edge("Publicated")]
        public List<Publication> Publications { get; set; }

        [Edge("Knows")]
        public List<Langue> Langues { get; set; }

        public CV()
        {
            DomainesDIntervention = new List<DomaineDIntervention>();
            FormationsAcademique = new List<FormationAcademique>();
            Certifications = new List<Certification>();

            Employeurs = new List<Employeur>();
            Perfectionnements = new List<Perfectionnement>();
            Conferences = new List<Conference>();
            Associations = new List<Association>();
            Publications = new List<Publication>();
            Langues = new List<Langue>();
        }
    }
}
