using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private Conseiller conseiller;
        private Utilisateur utilisateur;

        private Dictionary<string, int> DicMois;
        private IList list;

        DocumentClient documentClient;
        DocumentCollection documentCollection;

        public CVFactory()
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

            documentClient = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            documentCollection = documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("Graph_CV", "CVs"), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public Utilisateur CreateConseiller(List<XmlNode> Nodes)
        {
            SectionsExtractor CvSectionsExtractor = new SectionsExtractor();
            List<IXmlToken> matchTokens = new List<IXmlToken>();            

            matchTokens.Add(TextToken.CreateTextToken());
            matchTokens.Add(FormatationToken.CreateFormatationToken(new KeyValuePair<string, string>("w:val", "Titre1")));

            List<CVSection> Sections = CvSectionsExtractor.GetCVSections(Nodes, matchTokens, "IDENTIFICATION");

            conseiller = new Conseiller();
            AssemblerConseiller(Sections);

            return utilisateur;
            
        }

        private void AssemblerConseiller(List<CVSection> sections)
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

            aSection = sections.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifiant == "TECHNOLOGIES");
            if (aSection != null)
                AssamblerTechnologies(aSection);

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
            FonctionGraphRepository fonctionGraphRepository = new FonctionGraphRepository(documentClient, documentCollection);
            XmlDocNode identification = sectionIdentification.Nodes.First();

            Fonction fonction = new Fonction();
            CV cv = new CV();

            if (identification is XmlDocTable)
            {
                XmlDocParagraph paragraph = ((XmlDocTable)identification).GetParagraphsFromColumn(2).First();
                string[] identLines = paragraph.GetLinesText();

                utilisateur = new Utilisateur();
                utilisateur.Nom = identLines[0];
                fonction = fonctionGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", identLines[1] } });
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
            DomaineDInterventionGraphRepository repo = new DomaineDInterventionGraphRepository(documentClient, documentCollection);
            DomaineDIntervention domaine;

            domainesParagraphs.ForEach(x =>
            {

                domaine = DomaineDIntervention.CreateDomaineDIntervetion(x.GetParagraphText());
                domaine = repo.CreateIfNotExists(new Dictionary<string, object>{ { "Description", domaine.Description } });

                conseiller.DomaineDInterventions.Add(domaine);
            });
        }

        private void AssemblerCertifications(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(2).Skip(1).ToList();
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository(documentClient, documentCollection);
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository(documentClient, documentCollection);

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
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository(documentClient, documentCollection);
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository(documentClient, documentCollection);

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
            OrdreProfessionalGraphRepository ordreProfessionalGraphRepository = new OrdreProfessionalGraphRepository(documentClient, documentCollection);

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
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository(documentClient, documentCollection);
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository(documentClient, documentCollection);

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
            FormationScolaireGraphRepository formationScolaireGraphRepository = new FormationScolaireGraphRepository(documentClient, documentCollection);
            InstituitionGraphRepository instituitionGraphRepository = new InstituitionGraphRepository(documentClient, documentCollection);

            string nomInstituition, nomDiplome;

            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(1).Skip(1).ToList();
            formationParagraphs.RemoveAll(x => string.IsNullOrEmpty(x.GetParagraphText()));

            for (int i = 0; i < formationParagraphs.Count; i = i + 2)
            {
                nomDiplome = formationParagraphs[i].GetParagraphText();
                nomInstituition = formationParagraphs[i + 1].GetParagraphText();

                if (string.IsNullOrEmpty(nomDiplome) || string.IsNullOrEmpty(nomInstituition))
                    continue;

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
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository(documentClient, documentCollection);
            CategorieDeTechnologieGraphRepository categorieDeTechnologieGraphRepository = new CategorieDeTechnologieGraphRepository(documentClient, documentCollection);

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
                        mois = lineParagraphsColumn2[j].GetParagraphText();

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
            EmployeurGraphRepository employeurGraphRepository = new EmployeurGraphRepository(documentClient, documentCollection);

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
                Client client = AssemblerClient(x, emp);
                clients.Add(client);
            });
            
            return clients;
        }

        private Client AssemblerClient(CVSection clientSection, Employeur emp)
        {
            Client client = new Client();
            List<Mandat> mandats = new List<Mandat>();

            XmlDocParagraph emplDesc = (XmlDocParagraph)clientSection.Nodes.DefaultIfEmpty(null).FirstOrDefault(x => x is XmlDocParagraph);

            ClientGraphRepository clientGraphRepository = new ClientGraphRepository(documentClient, documentCollection);

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
            ProjetGraphRepository projetGraphRepository = new ProjetGraphRepository(documentClient, documentCollection);
            FonctionGraphRepository fonctionGraphRepository = new FonctionGraphRepository(documentClient, documentCollection);
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository(documentClient, documentCollection);

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
                    if(envergureText.Any(x => char.IsDigit(x)))
                        projet.Envergure = int.Parse(string.Join("", envergureText.Where(x => char.IsDigit(x)).ToArray()));
                }

                projet = projetGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Nom", projet.Nom }, { "Envergure", projet.Envergure } });

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
                        else {
                            fin = periode[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            if (fin.Length > 1)
                            {
                                mois = DicMois[RemoveAcentuation(fin[0].Trim().ToUpper())];
                                annee = int.Parse(fin[1].Trim());
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
            infoParagraphs.ForEach(x =>
            {
                if (x.GetParagraphText().ToUpper().Contains("ENVIRONNEMENT TECHNOLOGIQUE"))
                {
                    environnement = x.GetParagraphText().ToUpper().Replace("ENVIRONNEMENT TECHNOLOGIQUE", "").Trim();

                    if (environnement.First().Equals(':'))
                        environnement = environnement.Substring(1);

                    if (environnement.Last().Equals('.'))
                        environnement = environnement.Substring(0, environnement.Length - 1);

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
                    projet.Description += x.GetParagraphText();
                }

            });

            mandat.Projet = projet;

            return mandat;
        }

        private void AssemblerPerfectionnement(CVSection sectionPerfectionnement)
        {
            GenreGraphRepository genreGraphRepository = new GenreGraphRepository(documentClient, documentCollection);
            FormationGraphRepository formationGraphRepository = new FormationGraphRepository(documentClient, documentCollection);


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

                formation = formationGraphRepository.CreateIfNotExists(new Dictionary<string, object> { { "Description", secondColumn[i].GetParagraphText() }, { "AnAcquisition", annee} });                

                formation.Type = genre;

                conseiller.Formations.Add(formation);
                
            }
        }

        private void AssemblerLangues(CVSection langueSection)
        {
            LangueGraphRepository langueGraphRepository = new LangueGraphRepository(documentClient, documentCollection);

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
    }
}
