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
    public class CV
    {
        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        public List<DomaineDIntervention> DomainesDIntervention { get; set; }
        public List<FormationAcademique> FormationsAcademique { get; set; }
        public List<Certification> Certifications { get; set; }

        public List<Employeur> Employeurs { get; set; }
        public List<Perfectionnement> Perfectionnements { get; set; }
        public List<Conference> Conferences { get; set; }
        public List<Association> Associations { get; set; }
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
