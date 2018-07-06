using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        //private CV currentCV;
        string cvFileName;
        private Conseiller conseiller;
        private Utilisateur utilisateur;

        private CVSection aSection;

        private Dictionary<string, int> DicMois;
        //private IList list;

        public CVFactory(string fileName)
        {
            DicMois = new Dictionary<string, int>()
            {
                {"JANVIER", 1 },
                {"FEVRIER", 2 },
                {"MARS", 3 },
                {"AVRIL", 4 },
                {"MAI", 5 },
                {"JUIN", 6 },
                {"JUILLET", 7 },
                {"AOUT", 8 },
                {"SEPTEMBRE", 9 },
                {"OCTOBRE", 10 },
                {"NOVEMBRE", 11 },
                {"DECEMBRE", 12 }
            };
            cvFileName = fileName;
        }

        public Utilisateur CreateConseiller(List<XmlNode> Nodes)
        {
            SectionsExtractor CvSectionsExtractor = new SectionsExtractor();
            List<IXmlToken> matchTokens = new List<IXmlToken>();

            matchTokens.Add(TextToken.CreateTextToken());
            matchTokens.Add(FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Titre1")));

            List<CVSection> Sections = null;

            try
            {
                Sections = CvSectionsExtractor.GetCVSections(Nodes, matchTokens, "IDENTIFICATION");
                conseiller = new Conseiller();
                AssemblerConseiller(Sections);
            }
            catch (Exception ex)
            {
                WriteToErrorLog(ex);
            }

            return utilisateur;

        }

        private void Assemblage(List<CVSection> sections, string sectionName, List<Action<CVSection>> actions)
        {
            try
            {
                aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == sectionName);
                if (aSection != null)
                {
                    foreach (var action in actions)
                    {
                        try
                        {
                            action.Invoke(aSection);
                        }
                        catch (Exception ex)
                        {

                            WriteToErrorLog(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToErrorLog(ex);
            }
        }

        private void AssemblerConseiller(List<CVSection> sections)
        {

            Assemblage(sections, "IDENTIFICATION", new List<Action<CVSection>> { AssemblerBio });
            Assemblage(sections, "PRINCIPAUX DOMAINES D’INTERVENTION", new List<Action<CVSection>> { AssemberDomainesDIntervetion });
            Assemblage(sections, "FORMATION ACADÉMIQUE", new List<Action<CVSection>> { AssamblerFormations, AssemblerCertifications });
            Assemblage(sections, "TECHNOLOGIES", new List<Action<CVSection>> { AssamblerTechnologies });
            Assemblage(sections, "PERFECTIONNEMENT", new List<Action<CVSection>> { AssemblerPerfectionnement });
            Assemblage(sections, "CONFÉRENCES", new List<Action<CVSection>> { AssemblerConferences });
            Assemblage(sections, "PUBLICATIONS", new List<Action<CVSection>> { AssemblerPublications });
            Assemblage(sections, "ASSOCIATIONS", new List<Action<CVSection>> { AssemblerAssociations });
            Assemblage(sections, "LANGUES PARLÉES, ÉCRITES", new List<Action<CVSection>> { AssemblerLangues });


            sections.Where(x => x.Identifiant == "Titre1").ToList().ForEach(x =>
            {
                try
                {
                    AssemblerEmployeur(x);
                }
                catch (Exception ex)
                {
                    WriteToErrorLog(ex);
                }
            });
        }

        private void AssemblerBio(CVSection sectionIdentification)
        {
            FonctionGraphRepository fonctionGraphRepository = new FonctionGraphRepository();
            XmlDocNode identification = sectionIdentification.Nodes.First();

            Fonction fonction = new Fonction();
            CV cv = new CV();

            if (identification is XmlDocTable)
            {
                var paragraphs = ((XmlDocTable)identification).GetParagraphsFromColumn(2);
                List<string> identLines = new List<string>();
                if (paragraphs.Count() == 1)
                {
                    XmlDocParagraph paragraph = paragraphs.First<XmlDocParagraph>();
                    identLines.AddRange(paragraph.GetLinesText());
                }
                else
                {
                    foreach (var paragragh in paragraphs)
                    {
                        identLines.AddRange(paragragh.GetLinesText());
                    }

                }


                utilisateur = new Utilisateur();
                utilisateur.Nom = identLines.First();
                fonction = fonctionGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", identLines.Last() } });
            }

            string description = string.Empty;
            List<XmlDocParagraph> descriptionParagraphs = sectionIdentification.Nodes.Skip(2).Cast<XmlDocParagraph>().ToList();
            descriptionParagraphs.ForEach(x => description = string.Concat(description, x.GetParagraphText()));

            cv.ResumeExperience = description;
            cv.Status = StatusCV.Nouveau;


            utilisateur.Conseiller = conseiller;
            conseiller.Fonction = fonction;
            conseiller.CVs.Add(cv);
        }

        private void AssemberDomainesDIntervetion(CVSection sectionDomaines)
        {
            XmlDocTable domainesTable = sectionDomaines.Nodes.Skip(1).Cast<XmlDocTable>().First();
            List<XmlDocParagraph> domainesParagraphs = domainesTable.GetParagraphsFromColumns();
            DomaineDInterventionGraphRepository repo = new DomaineDInterventionGraphRepository();
            DomaineDIntervention domaine;

            domainesParagraphs.ForEach(x =>
            {

                domaine = DomaineDIntervention.CreateDomaineDIntervetion(x.GetParagraphText());
                domaine = repo.CreateIfNotExists(new Dictionary<string, object> { { "Description", domaine.Description } });

                conseiller.DomaineDInterventions.Add(domaine);
            });
        }

        private void AssemblerCertifications(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(2).Skip(1).ToList();
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository();
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository();

            formationParagraphs.ForEach(x =>
            {
                Formation formation;
                Genre genre = new Genre();

                genre.Descriminator = "Formation";
                genre.Description = "Certification";

                genre = genreGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", genre.Description }, { "Descriminator", genre.Descriminator } });

                string text = x.GetParagraphText();

                if (!string.IsNullOrEmpty(text))
                {
                    formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", text } });
                    formation.Type = genre;

                    conseiller.Formations.Add(formation);
                }

            });
        }

        private void AssemblerConferences(CVSection sectionConferences)
        {
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository();
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository();

            Genre genre;
            Formation formation;

            genre = genreGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", "Conference" }, { "Descriminator", "Formation" } });

            if (sectionConferences.Nodes.Any(x => x is XmlDocTable))
            {
                List<string> conferences = ((XmlDocTable)sectionConferences.Nodes.First(x => x is XmlDocTable)).GetAllLines();
                conferences.ForEach(x =>
                {
                    formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", x } });
                    formation.Type = genre;

                    conseiller.Formations.Add(formation);
                });
            }
            else
            {
                List<XmlDocParagraph> conferencesParagraphs = sectionConferences.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
                conferencesParagraphs.ForEach(x =>
                {
                    formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", x.GetParagraphText() } });
                    formation.Type = genre;

                    conseiller.Formations.Add(formation);
                });
            }
        }

        private void AssemblerAssociations(CVSection sectionAssociations)
        {
            OrdreProfessionalGraphRepository ordreProfessionalGraphRepository = new OrdreProfessionalGraphRepository();

            List<XmlDocParagraph> associationsParagraphs = sectionAssociations.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            associationsParagraphs.ForEach(x =>
            {
                OrdreProfessional ordre = new OrdreProfessional();
                ordre.Nom = x.GetParagraphText();
                ordre = ordreProfessionalGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", ordre.Nom } });

                conseiller.Associations.Add(ordre);
            });
        }

        private void AssemblerPublications(CVSection sectionPublications)
        {
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository();
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository();

            Genre genre = new Genre();
            genre.Descriminator = "Formation";
            genre.Description = "Publication";

            genre = genreGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", genre.Description }, { "Descriminator", genre.Descriminator } });

            List<XmlDocParagraph> publicationsParagraphs = sectionPublications.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList();
            publicationsParagraphs.ForEach(x =>
            {
                Formation formation;

                formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", x.GetParagraphText() } });
                formation.Type = genre;

                conseiller.Formations.Add(formation);
            });
        }

        private void AssamblerFormations(CVSection sectionFormation)
        {
            FormationScolaireGraphRepository formationScolaireGraphRepository = new FormationScolaireGraphRepository();
            InstituitionGraphRepository instituitionGraphRepository = new InstituitionGraphRepository();

            string nomInstituition = string.Empty, nomDiplome = string.Empty;

            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(1).Skip(1).ToList();
            formationParagraphs.RemoveAll(x => string.IsNullOrEmpty(x.GetParagraphText()));

            List<WordLine> Lines = new List<WordLine>();

            for (int i = 0; i < formationParagraphs.Count; i++)
                Lines.AddRange(formationParagraphs[i].GetLines());

            StringBuilder sb = new StringBuilder();
            WordLine currentLine;

            while (Lines.Count > 0)
            {
                currentLine = Lines.First();

                if (currentLine.IsBold())
                {
                    if (!string.IsNullOrEmpty(nomDiplome))
                    {
                        nomInstituition = sb.ToString();

                        FormationScolaire item;
                        Instituition inst;

                        inst = instituitionGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", nomInstituition } });

                        item = formationScolaireGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Diplome", nomDiplome }, { "Niveau", NiveauScolarite.Nule } });
                        item.Ecole = inst;

                        conseiller.FormationsScolaires.Add(item);

                        nomDiplome = string.Empty;
                        nomInstituition = string.Empty;
                        sb.Clear();
                    }

                    sb.Append(currentLine.GetText());
                    Lines.Remove(currentLine);
                }
                else
                {
                    if (string.IsNullOrEmpty(nomDiplome))
                    {
                        nomDiplome = sb.ToString();
                        sb.Clear();
                    }

                    sb.Append(currentLine.GetText());
                    Lines.Remove(currentLine);
                }
            }

            if (!string.IsNullOrEmpty(nomDiplome) && !string.IsNullOrEmpty(sb.ToString()))
            {
                nomInstituition = sb.ToString();

                FormationScolaire item;
                Instituition inst;

                inst = instituitionGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", nomInstituition } });

                item = formationScolaireGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Diplome", nomDiplome }, { "Niveau", NiveauScolarite.Nule } });
                item.Ecole = inst;

                conseiller.FormationsScolaires.Add(item);
            }
        }

        private void AssamblerTechnologies(CVSection sectionTechnologies)
        {
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository();
            CategorieDeTechnologieGraphRepository categorieDeTechnologieGraphRepository = new CategorieDeTechnologieGraphRepository();

            XmlDocTable tableTechnologies = (XmlDocTable)sectionTechnologies.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> lineParagraphsColumn1 = new List<XmlDocParagraph>(), lineParagraphsColumn2 = new List<XmlDocParagraph>();
            List<KeyValuePair<string, string>> listOfTech = new List<KeyValuePair<string, string>>();

            Technologie technologie;
            CategorieDeTechnologie categorie = null;
            string techNom, mois;

            for (int i = 1; i <= tableTechnologies.CountColumns(); i = i + 3)
            {
                lineParagraphsColumn1 = tableTechnologies.GetParagraphsFromColumn(i);
                lineParagraphsColumn2 = tableTechnologies.GetParagraphsFromColumn(i + 1);

                for (int j = 1; j < lineParagraphsColumn1.Count; j++)
                {
                    if (string.IsNullOrEmpty(lineParagraphsColumn1[j].GetParagraphText()))
                        continue;

                    if (string.IsNullOrEmpty(lineParagraphsColumn2[j].GetParagraphText()))
                    {
                        categorie = categorieDeTechnologieGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", lineParagraphsColumn1[j].GetParagraphText().Replace(":", "").Trim() } });
                    }
                    else
                    {
                        techNom = lineParagraphsColumn1[j].GetParagraphText();
                        mois = lineParagraphsColumn2[j].GetParagraphText().Replace(".", ",");

                        technologie = technologieGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", techNom } });
                        technologie.MoisDExperience = Convert.ToDouble(mois);

                        if (categorie != null)
                            technologie.Categorie = categorie;

                        conseiller.Technologies.Add(technologie);
                    }
                }
            }

        }

        private void AssemblerEmployeur(CVSection employeurSection)
        {
            XmlDocParagraph emplDesc = (XmlDocParagraph)employeurSection.Nodes.First(x => x is XmlDocParagraph);
            List<XmlDocParagraph> jobDescription = new List<XmlDocParagraph>();
            EmployeurGraphRepository employeurGraphRepository = new EmployeurGraphRepository();

            Employeur emp = new Employeur();
            string periode = string.Empty;
            string[] info = emplDesc.GetLinesWithTab(), periodeSplited;

            if (info.Length > 1)
            {
                emp = employeurGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", info[1] } });

                periode = info[0];
                periodeSplited = periode.Split("-");

                if (periodeSplited.Length > 1)
                {
                    emp.DateDebut = DateTime.Parse($"{periodeSplited[0].Trim()}-01-01");
                    emp.DateFin = DateTime.Parse($"{periodeSplited[1].Trim()}-12-31");
                }
                else
                {
                    emp.DateDebut = DateTime.Parse($"{periodeSplited[0].Trim()}-01-01");
                    emp.DateFin = DateTime.Parse($"{periodeSplited[0].Trim()}-12-31");
                }
            }
            else
            {
                emp = employeurGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", info[0] } });
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

            AssemblerClients(employeurSection, emp);

            conseiller.Employeurs.Add(emp);
        }

        private List<Client> AssemblerClients(CVSection employeurSection, Employeur emp)
        {
            List<Client> clients = new List<Client>();
            List<CVSection> clientSections = new List<CVSection>();
            List<XmlDocNode> empNodes = new List<XmlDocNode>();
            SectionsExtractor sectionsExtractor = new SectionsExtractor();

            clientSections.AddRange(sectionsExtractor.GetCVSections(employeurSection.GetOriginalNodes,
                new List<IXmlToken>() { FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Titre2")) }, "Titre2", true));

            clientSections.ForEach(x =>
            {
                try
                {
                    Client client = AssemblerClient(x, emp);
                    clients.Add(client);
                }
                catch (Exception ex)
                {
                    WriteToErrorLog(ex);
                }
            });

            return clients;
        }

        private Client AssemblerClient(CVSection clientSection, Employeur emp)
        {
            Client client = new Client();
            List<Mandat> mandats = new List<Mandat>();

            XmlDocParagraph emplDesc = (XmlDocParagraph)clientSection.Nodes.DefaultIfEmpty(null).FirstOrDefault(x => x is XmlDocParagraph);

            ClientGraphRepository clientGraphRepository = new ClientGraphRepository();

            if (emplDesc != null)
            {
                string[] info = emplDesc.GetLinesWithTab();
                client.Nom = string.Join(" ", info);
                client = clientGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", client.Nom } });
            }

            mandats.AddRange(AssemblerMandats(clientSection));
            mandats.ForEach(x =>
            {
                x.Projet.Client = client;
                x.Projet.SocieteDeConseil = emp;
            });

            conseiller.Mandats.AddRange(mandats);

            return client;
        }

        private List<Mandat> AssemblerMandats(CVSection clientSection)
        {
            List<Mandat> mandats = null;

            try
            {
                mandats = new List<Mandat>();
                List<XmlDocNode> mandatsNodes = new List<XmlDocNode>(),
                mandatNodes = new List<XmlDocNode>();

                mandatsNodes.AddRange(clientSection.Nodes.SkipWhile(x => x is XmlDocParagraph));

                do
                {
                    try
                    {
                        mandatNodes.Add(mandatsNodes.First(x => x is XmlDocTable));
                        mandatNodes.AddRange(mandatsNodes.SkipWhile(x => x is XmlDocTable).TakeWhile(x => x is XmlDocParagraph));

                        mandatsNodes.RemoveAll(x => mandatNodes.Contains(x));
                        Mandat mandat = AssemblerMandat(mandatNodes);

                        mandats.Add(mandat);

                        mandatNodes.Clear();
                    }
                    catch (Exception ex)
                    {
                        WriteToErrorLog(ex);
                        mandatNodes.Clear();
                    }

                } while (mandatsNodes.Count > 0);

            }
            catch (Exception ex)
            {
                WriteToErrorLog(ex);
            }

            return mandats;
        }

        private Mandat AssemblerMandat(List<XmlDocNode> mandatNodes)
        {
            ProjetGraphRepository projetGraphRepository = new ProjetGraphRepository();
            FonctionGraphRepository fonctionGraphRepository = new FonctionGraphRepository();
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository();

            Mandat mandat = new Mandat();
            Projet projet = new Projet();
            Technologie technologie = null;

            int parseInt = 0, mois, annee;
            string concatenatedString, envergureText, environnement, tech;
            string[] periode, debut, fin, splitedString, technologies;

            List<XmlDocParagraph> infoParagraphs = new List<XmlDocParagraph>(), infoParagraphsSecondColumn = new List<XmlDocParagraph>();

            XmlDocTable infoTable = (XmlDocTable)mandatNodes.First(x => x is XmlDocTable);
            infoParagraphs.AddRange(infoTable.GetParagraphsFromColumn(1));
            infoParagraphsSecondColumn.AddRange(infoTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < infoParagraphs.Count; i++)
            {
                concatenatedString = string.Empty;

                if (infoParagraphs[i].GetParagraphText().Contains("Projet"))
                    projet.Nom = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Mandat"))
                    mandat.Numero = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Envergure"))
                {
                    envergureText = infoParagraphsSecondColumn[i].GetParagraphText().Trim();
                    if (envergureText.Any(x => char.IsDigit(x)))
                        projet.Envergure = int.Parse(string.Join("", envergureText.Where(x => char.IsDigit(x)).ToArray()));
                }


                if (infoParagraphs[i].GetParagraphText().Contains("Fonction"))
                {
                    Fonction fonction;
                    fonction = fonctionGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", infoParagraphsSecondColumn[i].GetParagraphText() } });

                    mandat.Fonction = fonction;
                }

                if (infoParagraphs[i].GetParagraphText().Contains("Période"))
                {
                    periode = infoParagraphsSecondColumn[i].GetParagraphText().Split("à");
                    if (periode.Length > 1)
                    {
                        debut = periode[0].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        if (debut.Length > 1)
                        {
                            mois = DicMois[RemoveAcentuation(debut[0].Trim().ToUpper())];
                            annee = int.Parse(debut[1].Trim());
                            mandat.DateDebut = DateTime.Parse($"{annee}-{mois}-01");
                        }

                        if (periode[1].Contains("ce jour"))
                            mandat.DateFin = DateTime.MinValue;
                        else
                        {
                            fin = periode[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            if (fin.Length > 1)
                            {
                                mois = DicMois[RemoveAcentuation(fin[0].Trim().ToUpper())];
                                annee = int.Parse(fin[1].Sanitize());
                                mandat.DateFin = DateTime.Parse($"{annee}-{mois}-01");
                            }
                        }
                    }
                }

                if (infoParagraphs[i].GetParagraphText().Contains("Efforts"))
                {
                    splitedString = infoParagraphsSecondColumn[i].GetParagraphText().Split(" ");
                    parseInt = 0;

                    if (int.TryParse(splitedString[0], out parseInt))
                        mandat.Efforts = parseInt;
                }


                if (infoParagraphs[i].GetParagraphText().Contains("Référence"))
                    projet.NomReference = infoParagraphsSecondColumn[i].GetParagraphText();
            }


            infoParagraphsSecondColumn.Clear();
            infoParagraphsSecondColumn = null;

            infoParagraphs.Clear();
            infoParagraphs = mandatNodes.SkipWhile(x => x is XmlDocTable).Cast<XmlDocParagraph>().ToList();
            foreach (var x in infoParagraphs)
            {
                if (x.GetParagraphText().ToUpper().StartsWith("ENVIRONNEMENT TECHNOLOGIQUE"))
                {
                    environnement = x.GetParagraphText().ToUpper().Replace("ENVIRONNEMENT TECHNOLOGIQUE", "").Trim();

                    if (environnement.Count() == 0)
                        continue;

                    if (environnement.First().Equals(':'))
                        environnement = environnement.Substring(1);

                    if (environnement.Count() == 0)
                        continue;

                    if (environnement.Last().Equals('.'))
                        environnement = environnement.Substring(0, environnement.Length - 1);

                    if (environnement.Count() == 0)
                        continue;

                    technologies = environnement.Split(",");

                    for (int i = 0; i < technologies.Length; i++)
                    {
                        tech = technologies[i].Trim();
                        technologie = technologieGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", tech } });

                        if (technologie != null)
                            projet.Technologies.Add(technologie);
                    }
                }
                else
                {
                    mandat.Description += x.GetParagraphText();
                }
            }

            projet.Description = mandat.Description;

            mandat.Titre = projet.Nom;
            mandat.Projet = projet;

            projet.DateDebut = mandat.DateDebut;
            projet.DateFin = mandat.DateFin;

            return mandat;
        }

        private void AssemblerPerfectionnement(CVSection sectionPerfectionnement)
        {
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository();
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository();


            Genre genre = new Genre();
            genre.Descriminator = "Formation";
            genre.Description = "Perfectionnement";

            genre = genreGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", genre.Description }, { "Descriminator", genre.Descriminator } });

            int annee = 0;

            XmlDocTable perfcTable = (XmlDocTable)sectionPerfectionnement.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> firstColumn = new List<XmlDocParagraph>(), secondColumn = new List<XmlDocParagraph>();

            firstColumn.AddRange(perfcTable.GetParagraphsFromColumn(1));
            secondColumn.AddRange(perfcTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < firstColumn.Count; i++)
            {
                Formation formation;

                if (!int.TryParse(firstColumn[i].GetParagraphText(), out annee))
                    annee = 0;

                formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", secondColumn[i].GetParagraphText() }, { "AnAcquisition", annee } });

                formation.Type = genre;

                conseiller.Formations.Add(formation);

            }
        }

        private void AssemblerLangues(CVSection langueSection)
        {
            LangueGraphRepository langueGraphRepository = new LangueGraphRepository();

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
                curLangue = langueGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", curLangue.Nom } });

                if (section.Nodes.Skip(1).Count() > 0)
                {
                    foreach (XmlDocParagraph langueNiveau in section.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList())
                    {
                        string[] niveau = langueNiveau.GetParagraphText().Split(':');

                        if (niveau[0].Contains("Parlé"))
                            curLangue.Parle = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim().Replace("-", "").ToCamelCase());

                        if (niveau[0].Contains("Écrit"))
                            curLangue.Ecrit = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim().Replace("-", "").ToCamelCase());

                        if (niveau[0].Contains("Lu"))
                            curLangue.Lu = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim().Replace("-", "").ToCamelCase());
                    }
                }
                else
                {
                    curLangue.Parle = curLangue.Ecrit = curLangue.Lu = Niveau.Avancé;
                }

                langues.Add(curLangue);
            }

            conseiller.Langues.AddRange(langues);
        }

        public string RemoveAcentuation(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void WriteToErrorLog(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter("Errors.log", true))
            {
                sw.WriteLine(cvFileName);
                sw.WriteLine(ex.Message);
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine("=========================X=========================");
                sw.WriteLine(string.Empty);
            }
        }
    }
}
