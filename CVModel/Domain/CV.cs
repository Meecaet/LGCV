using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XmlHandler.XmlEntities;

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

        public void AssemblerCV(List<CVSection> sections)
        {
            CVSection aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "IDENTIFICATION");
            if (aSection != null)
                AssemblerIndentification(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "PRINCIPAUX DOMAINES D’INTERVENTION");
            if (aSection != null)
                AssemberDomainesDIntervetion(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "FORMATION ACADÉMIQUE");
            if (aSection != null)
            {
                FormationsAcademique.AddRange(FormationAcademique.AssamblerFormationEtCertifications(aSection));
                AssemblerCertifications(aSection);
            }
                        
            sections.Where(x => x.Identifiant == "Titre1").ToList().ForEach(x => 
            {
                Employeur emp = new Employeur();
                emp.AssemblerEmployeur(x);

                this.Employeurs.Add(emp);
            });

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "PERFECTIONNEMENT");
            if (aSection != null)
                Perfectionnements.AddRange(Perfectionnement.AssemblerPerfectionnement(aSection));
            
            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "CONFÉRENCES");
            if (aSection != null)
                AssemblerConferences(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "PUBLICATIONS");
            if (aSection != null)
                AssemblerPublications(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "ASSOCIATIONS");
            if (aSection != null)
                AssemblerAssociations(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "LANGUES PARLÉES, ÉCRITES");
            if(aSection != null)
                Langues.AddRange(Langue.AssemblerLangues(aSection));
        }

        private void AssemberDomainesDIntervetion(CVSection sectionDomaines)
        {
            XmlDocTable domainesTable = sectionDomaines.Nodes.Skip(1).Cast<XmlDocTable>().First();
            List<XmlDocParagraph> domainesParagraphs = domainesTable.GetParagraphsFromColumns();

            domainesParagraphs.ForEach(x => 
            {
                DomaineDIntervention.Add(x.GetParagraphText());
            });
        }

        private void AssemblerIndentification(CVSection sectionIdentification)
        {
            XmlDocNode identification = sectionIdentification.Nodes.First();

            if (identification is XmlDocTable)
            {
                XmlDocParagraph paragraph = ((XmlDocTable)identification).GetParagraphsFromColumn(2).First();
                string[] identLines = paragraph.GetLinesText();

                this.Nom = identLines[0];
                this.Titre = identLines[1];
            }

            string description = string.Empty;
            List<XmlDocParagraph> descriptionParagraphs = sectionIdentification.Nodes.Skip(2).Cast<XmlDocParagraph>().ToList();
            descriptionParagraphs.ForEach(x => description = string.Concat(description, x.GetParagraphText()));

            this.Description = description;
        }

        private void AssemblerCertifications(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(2).Skip(1).ToList();
            formationParagraphs.ForEach(x => 
            {
                string text = x.GetParagraphText();

                if (!string.IsNullOrEmpty(text))
                    Certifications.Add(text);
            });
        }

        private void AssemblerConferences(CVSection sectionConferences)
        {
            if (sectionConferences.Nodes.Any(x => x is XmlDocTable))
            {
                List<string> conferences = ((XmlDocTable)sectionConferences.Nodes.First(x => x is XmlDocTable)).GetAllLines();
                conferences.ForEach(x => Conferences.Add(x));
            }
            else
            {
                List<XmlDocParagraph> conferencesParagraphs = sectionConferences.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
                conferencesParagraphs.ForEach(x => Conferences.Add(x.GetParagraphText()));
            }
        }

        private void AssemblerAssociations(CVSection sectionAssociations)
        {
            List<XmlDocParagraph> associationsParagraphs = sectionAssociations.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            associationsParagraphs.ForEach(x => Associations.Add(x.GetParagraphText()));
        }

        private void AssemblerPublications(CVSection sectionPublications)
        {
            List<XmlDocParagraph> publicationsParagraphs = sectionPublications.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            publicationsParagraphs.ForEach(x => Publications.Add(x.GetParagraphText()));
        }

    }
}
