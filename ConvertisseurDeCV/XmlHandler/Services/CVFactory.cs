using CVModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlHandler.XmlEntities;
using XmlHandler.XmlIdentification;

namespace XmlHandler.Services
{
    /// <summary>
    /// Fait l'assemblage d'un objet CV depuis une liste de nœuds
    /// </summary>
    public class CVFactory
    {
        private CV currentCV;

        public CV CreateCV(List<XmlNode> Nodes)
        {
            SectionsExtractor CvSectionsExtractor = new SectionsExtractor();
            List<IXmlToken> matchTokens = new List<IXmlToken>();            

            matchTokens.Add(TextToken.CreateTextToken());
            matchTokens.Add(FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Titre1")));

            List<CVSection> Sections = CvSectionsExtractor.GetCVSections(Nodes, matchTokens, "IDENTIFICATION");
            currentCV = new CV();

            AssemblerCV(Sections);

            return currentCV;
            
        }

        private void AssemblerCV(List<CVSection> sections)
        {
            CVSection aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "IDENTIFICATION");
            if (aSection != null)
                AssemblerBio(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "PRINCIPAUX DOMAINES D’INTERVENTION");
            if (aSection != null)
                AssemberDomainesDIntervetion(aSection);

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "FORMATION ACADÉMIQUE");
            if (aSection != null)
            {
                AssamblerFormations(aSection);
                AssemblerCertifications(aSection);
            }

            sections.Where(x => x.Identifiant == "Titre1").ToList().ForEach(x => AssemblerEmployeur(x));

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "PERFECTIONNEMENT");
            if (aSection != null)
                AssemblerPerfectionnement(aSection);

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
            if (aSection != null)
                AssemblerLangues(aSection);
        }

        private void AssemblerBio(CVSection sectionIdentification)
        {
            XmlDocNode identification = sectionIdentification.Nodes.First();

            if (identification is XmlDocTable)
            {
                XmlDocParagraph paragraph = ((XmlDocTable)identification).GetParagraphsFromColumn(2).First();
                string[] identLines = paragraph.GetLinesText();

                currentCV.Nom = identLines[0];
                currentCV.Titre = identLines[1];
            }

            string description = string.Empty;
            List<XmlDocParagraph> descriptionParagraphs = sectionIdentification.Nodes.Skip(2).Cast<XmlDocParagraph>().ToList();
            descriptionParagraphs.ForEach(x => description = string.Concat(description, x.GetParagraphText()));

            currentCV.Description = description;
        }

        private void AssemberDomainesDIntervetion(CVSection sectionDomaines)
        {
            XmlDocTable domainesTable = sectionDomaines.Nodes.Skip(1).Cast<XmlDocTable>().First();
            List<XmlDocParagraph> domainesParagraphs = domainesTable.GetParagraphsFromColumns();

            domainesParagraphs.ForEach(x =>
            {
                currentCV.DomainesDIntervention.Add(DomaineDIntervention.CreateDomaineDIntervetion(x.GetParagraphText()));
            });
        }

        private void AssemblerCertifications(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(2).Skip(1).ToList();
            formationParagraphs.ForEach(x =>
            {
                string text = x.GetParagraphText();

                if (!string.IsNullOrEmpty(text))
                    currentCV.Certifications.Add(Certification.CreateCertification(text));
            });
        }

        private void AssemblerConferences(CVSection sectionConferences)
        {
            if (sectionConferences.Nodes.Any(x => x is XmlDocTable))
            {
                List<string> conferences = ((XmlDocTable)sectionConferences.Nodes.First(x => x is XmlDocTable)).GetAllLines();
                conferences.ForEach(x => currentCV.Conferences.Add(Conference.CreateConference(x)));
            }
            else
            {
                List<XmlDocParagraph> conferencesParagraphs = sectionConferences.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
                conferencesParagraphs.ForEach(x => currentCV.Conferences.Add(Conference.CreateConference(x.GetParagraphText())));
            }
        }

        private void AssemblerAssociations(CVSection sectionAssociations)
        {
            List<XmlDocParagraph> associationsParagraphs = sectionAssociations.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            associationsParagraphs.ForEach(x => currentCV.Associations.Add(Association.CreateAssociation(x.GetParagraphText())));
        }

        private void AssemblerPublications(CVSection sectionPublications)
        {
            List<XmlDocParagraph> publicationsParagraphs = sectionPublications.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            publicationsParagraphs.ForEach(x => currentCV.Publications.Add(Publication.CreatePublication(x.GetParagraphText())));
        }

        private void AssamblerFormations(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(1).Skip(1).ToList();
            formationParagraphs.RemoveAll(x => string.IsNullOrEmpty(x.GetParagraphText()));

            for (int i = 0; i < formationParagraphs.Count; i = i + 2)
            {
                FormationAcademique item = new FormationAcademique();
                item.Titre = formationParagraphs[i].GetParagraphText();
                item.Instituition = formationParagraphs[i + 1].GetParagraphText();

                if (string.IsNullOrEmpty(item.Titre) || string.IsNullOrEmpty(item.Instituition))
                    continue;

                currentCV.FormationsAcademique.Add(item);
            }
        }

        private void AssemblerEmployeur(CVSection employeurSection)
        {
            XmlDocParagraph emplDesc = (XmlDocParagraph)employeurSection.Nodes.First(x => x is XmlDocParagraph);
            List<XmlDocParagraph> jobDescription = new List<XmlDocParagraph>();

            Employeur emp = new Employeur();

            string[] info = emplDesc.GetLinesWithTab();

            if (info.Length > 1)
            {
                emp.Periode = info[0];
                emp.Nom = info[1];
            }
            else
            {
                emp.Nom = info[0];
            }

            jobDescription.AddRange(employeurSection.Nodes.Skip(1).TakeWhile(x => x is XmlDocParagraph).Cast<XmlDocParagraph>());
            if (jobDescription.Count > 0)
            {
                jobDescription.RemoveAt(jobDescription.Count - 1);
                employeurSection.Nodes.Remove(emplDesc);
                employeurSection.Nodes.RemoveAll(x => jobDescription.Contains(x));

                if (jobDescription.Count > 0)
                {
                    jobDescription.ForEach(x =>
                    {
                        emp.DescriptionDuTravail += x.GetParagraphText();
                    });
                }
            }
            else
                emp.DescriptionDuTravail = string.Empty;

            emp.Clients.AddRange(AssemblerClients(employeurSection));
            currentCV.Employeurs.Add(emp);
        }

        private List<Client> AssemblerClients(CVSection employeurSection)
        {
            List<Client> clients = new List<Client>();
            List<CVSection> clientSections = new List<CVSection>();
            List<XmlDocNode> empNodes = new List<XmlDocNode>();
            SectionsExtractor sectionsExtractor = new SectionsExtractor();

            clientSections.AddRange(sectionsExtractor.GetCVSections(employeurSection.GetOriginalNodes, 
                new List<IXmlToken>() { FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Titre2")) }, "Titre2", true));

            clientSections.ForEach(x =>
            {
                Client client = AssemblerClient(x);
                clients.Add(client);
            });
            
            return clients;
        }

        private Client AssemblerClient(CVSection clientSection)
        {
            Client client = new Client();
            XmlDocParagraph emplDesc = (XmlDocParagraph)clientSection.Nodes.DefaultIfEmpty(null).FirstOrDefault(x => x is XmlDocParagraph);

            if (emplDesc != null)
            {
                string[] info = emplDesc.GetLinesWithTab();
                client.Nom = string.Join(" ", info);
            }

            client.Mandats.AddRange(AssemblerMandats(clientSection));
            return client;
        }

        private List<Mandat> AssemblerMandats(CVSection clientSection)
        {
            List<Mandat> mandats = new List<Mandat>();
            List<XmlDocNode> mandatsNodes = new List<XmlDocNode>(),
            mandatNodes = new List<XmlDocNode>();

            mandatsNodes.AddRange(clientSection.Nodes.SkipWhile(x => x is XmlDocParagraph));

            do
            {
                mandatNodes.Add(mandatsNodes.First(x => x is XmlDocTable));
                mandatNodes.AddRange(mandatsNodes.SkipWhile(x => x is XmlDocTable).TakeWhile(x => x is XmlDocParagraph));

                mandatsNodes.RemoveAll(x => mandatNodes.Contains(x));
                Mandat mandat = AssemblerMandat(mandatNodes);
                mandats.Add(mandat);

                mandatNodes.Clear();

            } while (mandatsNodes.Count > 0);

            return mandats;
        }

        private Mandat AssemblerMandat(List<XmlDocNode> mandatNodes)
        {
            Mandat mandat = new Mandat();
            List<XmlDocParagraph> infoParagraphs = new List<XmlDocParagraph>(), infoParagraphsSecondColumn = new List<XmlDocParagraph>();

            XmlDocTable infoTable = (XmlDocTable)mandatNodes.First(x => x is XmlDocTable);
            infoParagraphs.AddRange(infoTable.GetParagraphsFromColumn(1));
            infoParagraphsSecondColumn.AddRange(infoTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < infoParagraphs.Count; i++)
            {
                if (infoParagraphs[i].GetParagraphText().Contains("Projet"))
                    mandat.Projet = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Mandat"))
                    mandat.Numero = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Envergure"))
                    mandat.Envenrgure = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Fonction"))
                    mandat.Fonction = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Période"))
                    mandat.Periode = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Efforts"))
                    mandat.Efforts = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Référence"))
                    mandat.Reference = infoParagraphsSecondColumn[i].GetParagraphText();
            }

            infoParagraphsSecondColumn.Clear();
            infoParagraphsSecondColumn = null;

            infoParagraphs.Clear();
            infoParagraphs = mandatNodes.SkipWhile(x => x is XmlDocTable).Cast<XmlDocParagraph>().ToList();
            infoParagraphs.ForEach(x => mandat.Description += x.GetParagraphText());

            return mandat;
        }

        private void AssemblerPerfectionnement(CVSection sectionPerfectionnement)
        {
            List<Perfectionnement> perfectionnements = new List<Perfectionnement>();
            XmlDocTable perfcTable = (XmlDocTable)sectionPerfectionnement.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> firstColumn = new List<XmlDocParagraph>(), secondColumn = new List<XmlDocParagraph>();

            firstColumn.AddRange(perfcTable.GetParagraphsFromColumn(1));
            secondColumn.AddRange(perfcTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < firstColumn.Count; i++)
            {
                Perfectionnement Perfc = new Perfectionnement();
                Perfc.An = firstColumn[i].GetParagraphText();
                Perfc.Description = secondColumn[i].GetParagraphText();

                perfectionnements.Add(Perfc);
            }

            currentCV.Perfectionnements.AddRange(perfectionnements);
        }

        private void AssemblerLangues(CVSection langueSection)
        {
            List<Langue> langues = new List<Langue>();
            List<CVSection> langueSections;
            
            SectionsExtractor extractor = new SectionsExtractor();
            langueSections = extractor.GetCVSections(langueSection.GetOriginalNodes.Skip(1).ToList(), 
                new List<IXmlToken>() { FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Puce1")) }, "w:b", true);

            foreach (CVSection section in langueSections)
            {
                Langue curLangue = new Langue();
                XmlDocParagraph langueNom = (XmlDocParagraph)section.Nodes.First(x => x is XmlDocParagraph);

                curLangue.Nom = langueNom.GetParagraphText();

                if (section.Nodes.Skip(1).Count() > 0)
                {
                    foreach (XmlDocParagraph langueNiveau in section.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList())
                    {
                        string[] niveau = langueNiveau.GetParagraphText().Split(':');

                        if (niveau[0].Contains("Parlé"))
                            curLangue.Parle = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());

                        if (niveau[0].Contains("Écrit"))
                            curLangue.Ecrit = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());

                        if (niveau[0].Contains("Lu"))
                            curLangue.Lu = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());
                    }
                }
                else
                {
                    curLangue.Parle = curLangue.Ecrit = curLangue.Lu = Niveau.Avancé;
                }

                langues.Add(curLangue);
            }

            currentCV.Langues.AddRange(langues);
        }
    }
}
