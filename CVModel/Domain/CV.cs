using CVModel.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.Domain
{
    public class CV
    {
        private XmlNamespaceManager xmlNamespaceManager;

        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        public List<string> DomaineDIntervention { get; set; }
        public FormationAcademique FormationAcademique { get; set; }
        public List<string> Certifications { get; set; }

        public List<Employeur> Employeurs { get; set; }
        public List<string> Perfectionnement { get; set; }
        public List<string> Conferences { get; set; }
        public List<Lange> Langes { get; set; }

        public CV()
        {
            DomaineDIntervention = new List<string>();
            FormationAcademique = new FormationAcademique();
            Certifications = new List<string>();

            Employeurs = new List<Employeur>();
            Perfectionnement = new List<string>();
            Conferences = new List<string>();
            Langes = new List<Lange>();
        }

        public void AssemblerCV(List<CVSection> sections)
        {
            CVSection aSection = sections.First(x => x.Identifiant == "IDENTIFICATION");
            AssemblerIndentification(aSection);

            aSection = sections.First(x => x.Identifiant == "PRINCIPAUX DOMAINES D’INTERVENTION");
            AssemberDomainesDIntervetion(aSection);

            aSection = sections.First(x => x.Identifiant == "FORMATION ACADÉMIQUE");
            FormationAcademique.AssamblerFormationEtCertifications(aSection);
            AssemblerCertifications(aSection);

            sections.Where(x => x.Identifiant == "Titre1").ToList().ForEach(x => 
            {
                Employeur emp = new Employeur();
                emp.AssemblerEmployeur(x);

                this.Employeurs.Add(emp);
            });
        }

        private void AssemberDomainesDIntervetion(CVSection sectionDomaines)
        {
            XmlDocTable domainesTable = sectionDomaines.Nodes.Skip(1).Cast<XmlDocTable>().First();
            List<XmlDocParagraph> domainesParagraphs = domainesTable.GetParagraphsFromColumn(1);

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

    }
}
