using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CVModel.Domain
{
    [Serializable]
    public class CV
    {
        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        [XmlArrayItem(ElementName = "Domaine")]
        public List<string> DomaineDIntervention { get; set; }
        public List<FormationAcademique> FormationsAcademique { get; set; }
        [XmlArrayItem(ElementName = "Certification")]
        public List<string> Certifications { get; set; }

        public List<Employeur> Employeurs { get; set; }
        public List<Perfectionnement> Perfectionnements { get; set; }
        [XmlArrayItem(ElementName = "Conference")]
        public List<string> Conferences { get; set; }
        [XmlArrayItem(ElementName = "Association")]
        public List<string> Associations { get; set; }
        [XmlArrayItem(ElementName = "Publication")]
        public List<string> Publications { get; set; }
        public List<Langue> Langues { get; set; }

        public CV()
        {
            DomaineDIntervention = new List<string>();
            FormationsAcademique = new List<FormationAcademique>();
            Certifications = new List<string>();

            Employeurs = new List<Employeur>();
            Perfectionnements = new List<Perfectionnement>();
            Conferences = new List<string>();
            Associations = new List<string>();
            Publications = new List<string>();
            Langues = new List<Langue>();
        }

        public void AssemberDomainesDIntervetion(List<string> domaines)
        {
            DomaineDIntervention.AddRange(domaines);
        }

        public void AssemblerBio(string Nom, string Titre, string description)
        {
            this.Nom = Nom;
            this.Titre = Titre;
            this.Description = description;
        }

        public void AssemblerCertifications(List<string> certifications)
        {
            Certifications.AddRange(certifications);
        }

        public void AssemblerConferences(List<string> conferences)
        {
            Conferences.AddRange(conferences);
        }

        public void AssemblerAssociations(List<string> associations)
        {
            Associations.AddRange(associations);
        }

        public void AssemblerPublications(List<string> publications)
        {
            Publications.AddRange(publications);
        }

       
       

    }
}
