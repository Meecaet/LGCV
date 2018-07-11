using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("CVPoc")]
    public class CVPocController : Controller
    {
        private UtilisateurGraphRepository UtilisateurDepot;

        private CVViewModel CreateDummyCVViewModel()
        {
            var model = new CVViewModel()
            {
                GraphIdUtilisateur = "0d8e017c-b21e-4b5c-b0bb-2cc09d8a3758",
                Biographie = @"M. Albert Wesker est un conseiller sénior titulaire d’un baccalauréat en informatique avec plus de vingt cinq (25) années d’expérience à son actif. Il œuvre principalement dans le cadre d’amélioration continue des processus d’affaires. Fort de ses connaissances en méthodologies Agile, Lean, Six Sigma, Productivité+ et ITIL, il conseille les entreprises dans leurs projets de changements organisationnels : de la définition des objectifs d’affaires jusqu’à l’opérationnalisation des processus-métier. Au cours de années,
                                il a agi à titre d’architecte du travail et d’analyste d’affaires des services TI dans des projets d’infrastructures technologiques diversifiés et complexes,
                                nécessitant une organisation du travail rigoureuse,
                                efficace et cohérente.Ses interventions visent à identifier,
                                analyser et documenter les impacts de la mise en place de solutions et de technologies.Ses travaux en ré - ingénierie des processus d’affaires et technologiques et façons de faire ont pour objectif de recommander des orientations qui contribueront à choisir les meilleures solutions,
                                faciliteront leur intégration et optimiseront les coûts et ce,
                                afin d’assurer et d’améliorer la qualité des services offerts aux clientèles.",
                Fonction = "Analyste devéloppeur .Net",
                Certifications =
                {
                    new CertificationViewModel()
                    {
                        Description = "ASP .NET MVC"
                    },
                    new CertificationViewModel()
                    {
                        Description = "Oracle 11G"
                    },
                    new CertificationViewModel()
                    {
                        Description = "Java"
                    }
                },
                FormationsAcademique = {
                    new FormationAcademiqueViewModel()
                    {
                        Annee = 1999,
                        Diplome = "Ingenieur informaticien",
                        Etablissement = "Université Laval"
                    },
                    new FormationAcademiqueViewModel()
                    {
                        Annee = 1999,
                        Diplome = "Maîtrise dans Banques de données",
                        Etablissement = "Université Laval"
                    }
                },
                Langues = {
                    new LangueViewModel
                    {
                        Nom = "Français",
                        NiveauEcrit = "Avancé",
                        NiveauLu = "Avancé",
                        NiveauParle = "Intermediare"
                    },
                    new LangueViewModel
                    {
                        Nom = "Espagnol",
                        NiveauEcrit = "Avancé",
                        NiveauLu = "Avancé",
                        NiveauParle = "Avancé"
                    },
                    new LangueViewModel
                    {
                        Nom = "Anglais",
                        NiveauEcrit = "Avancé",
                        NiveauLu = "Avancé",
                        NiveauParle = "Intermediare"
                    }
                },
                Nom = "Wesker",
                Prenom = "Albert",
                Technologies =
                {
                    new TechnologieViewModel
                    {
                        Description = "C#",
                        Mois = 72
                    },
                    new TechnologieViewModel
                    {
                        Description = "Java",
                        Mois = 30
                    },
                    new TechnologieViewModel
                    {
                        Description = "Sql Server",
                        Mois = 50
                    }
                },
                Perfectionnements =
                {
                    new PerfectionnementViewModel
                    {
                        Description = "Angular 2+"
                    },
                    new PerfectionnementViewModel
                    {
                        Description = "Azure"
                    },
                    new PerfectionnementViewModel
                    {
                        Description = "Devops"
                    }
                },
                DomainesDIntervention =
                {
                    new DomaineDInterventionViewModel
                    {
                        Description = "D1"
                    },
                    new DomaineDInterventionViewModel
                    {
                        Description = "D2"
                    }
                },
                Mandats =
                {
                    new MandatViewModel
                    {
                        CellulaireReference = "418 261 3369",
                        ContexteProjet = "Le projet UNIDOS, mis sur pied par le gouvernement de la Colombie, a pour but d’éradiquer la pauvreté extrême dans le pays. Ce mandat a consisté à concevoir et mettre en œuvre un système informatique de gestion des interventions dans le cadre d’un programme gouvernemental d’aide aux familles vulnérables visant à améliorer la situation économique, sociale, éducative et même résoudre des problèmes de violence intrafamiliale. Ce système permettait aux agents du gouvernement (intervenants sociaux) qui rendaient visite aux familles d’enregistrer les données recueillies et ensuite, de les traiter pour réaliser une stratégie d’aide individuelle et personnalisée pour chaque famille.",
                        CourrielReference = "unidos@umbrella.com",
                        DebutMandat = DateTime.Now.AddDays(-1000),
                        FinMandat = DateTime.Now.AddDays(-500),
                        DebutProjet = DateTime.Now.AddDays(-1000),
                        FinProjet = DateTime.Now.AddDays(-500),
                        Efforts = 50,
                        Envergure = 1500,
                        Fonction = "Analyste devéloppeur .Net",
                        FonctionReference = "Architect",
                        NomClient = "Umbrella Corporation",
                        NomEntreprise = "Umbrella Corporation",
                        NomReference = "James Markus",
                        TelephoneReference = "418 631 6523",
                        NumeroMandat = 3,
                        TitreMandat = "Inventoire des armes biologiques B.O.W",
                        TitreProjet = "Inventoire des armes biologiques B.O.W",
                        PorteeDesTravaux = "",
                        Taches = {
                            new TacheViewModel
                            {
                                Description = "Conseil auprès de l’équipe d’implantation"
                            },
                            new TacheViewModel
                            {
                                Description = "Définition de la solution cible"
                            },
                            new TacheViewModel
                            {
                                Description = "Présentation du modèle de livraison par phases"
                            },
                            new TacheViewModel
                            {
                                Description = "Élaboration d'un plan d'évolution des infrastructures technologiques"
                            }
                        },
                        Technologies =
                        {
                            new TechnologieViewModel
                            {
                                Description = "Asp .net",
                                Mois = 15
                            },
                            new TechnologieViewModel
                            {
                                Description = "C#",
                                Mois = 20
                            },
                            new TechnologieViewModel
                            {
                                Description = "Phyton",
                                Mois = 15
                            }
                        }
                    },
                    new MandatViewModel
                    {
                        CellulaireReference = "418 261 3369",
                        ContexteProjet = "Le projet UNIDOS, mis sur pied par le gouvernement de la Colombie, a pour but d’éradiquer la pauvreté extrême dans le pays. Ce mandat a consisté à concevoir et mettre en œuvre un système informatique de gestion des interventions dans le cadre d’un programme gouvernemental d’aide aux familles vulnérables visant à améliorer la situation économique, sociale, éducative et même résoudre des problèmes de violence intrafamiliale. Ce système permettait aux agents du gouvernement (intervenants sociaux) qui rendaient visite aux familles d’enregistrer les données recueillies et ensuite, de les traiter pour réaliser une stratégie d’aide individuelle et personnalisée pour chaque famille.",
                        CourrielReference = "unidos@umbrella.com",
                        DebutMandat = DateTime.Now.AddDays(-1000),
                        FinMandat = DateTime.Now.AddDays(-500),
                        DebutProjet = DateTime.Now.AddDays(-1000),
                        FinProjet = DateTime.Now.AddDays(-500),
                        Efforts = 50,
                        Envergure = 1500,
                        Fonction = "Analyste devéloppeur .Net",
                        FonctionReference = "Architect",
                        NomClient = "Walmart",
                        NomEntreprise = "Walmart",
                        NomReference = "James Markus",
                        TelephoneReference = "418 631 6523",
                        NumeroMandat = 2,
                        TitreMandat = "Securité",
                        TitreProjet = "Systéme de securité biometrique",
                        PorteeDesTravaux = "",
                        Taches = {
                            new TacheViewModel
                            {
                                Description = "Conseil auprès de l’équipe d’implantation"
                            },
                            new TacheViewModel
                            {
                                Description = "Définition de la solution cible"
                            },
                            new TacheViewModel
                            {
                                Description = "Présentation du modèle de livraison par phases"
                            },
                            new TacheViewModel
                            {
                                Description = "Élaboration d'un plan d'évolution des infrastructures technologiques"
                            }
                        },
                        Technologies =
                        {
                            new TechnologieViewModel
                            {
                                Description = "Visual basic .net",
                                Mois = 15
                            },
                            new TechnologieViewModel
                            {
                                Description = "Azure",
                                Mois = 20
                            },
                            new TechnologieViewModel
                            {
                                Description = "Linux",
                                Mois = 15
                            }
                        }
                    },
                    new MandatViewModel
                    {
                        CellulaireReference = "418 261 3369",
                        ContexteProjet = "PROJET DUMMY.",
                        CourrielReference = "unidos@umbrella.com",
                        DebutMandat = DateTime.Now.AddDays(-1000),
                        FinMandat = DateTime.Now.AddDays(-500),
                        DebutProjet = DateTime.Now.AddDays(-1000),
                        FinProjet = DateTime.Now.AddDays(-500),
                        Efforts = 50,
                        Envergure = 1500,
                        Fonction = "Analyste devéloppeur .Net",
                        FonctionReference = "Architect",
                        NomClient = "Walmart",
                        NomEntreprise = "Walmart",
                        NomReference = "James Markus",
                        TelephoneReference = "418 631 6523",
                        NumeroMandat = 1,
                        TitreMandat = "Securité",
                        TitreProjet = "Systéme de securité biometrique",
                        PorteeDesTravaux = "",
                        Taches = {
                            new TacheViewModel
                            {
                                Description = "Conseil auprès de l’équipe d’implantation"
                            },
                            new TacheViewModel
                            {
                                Description = "Définition de la solution cible"
                            },
                            new TacheViewModel
                            {
                                Description = "Présentation du modèle de livraison par phases"
                            },
                            new TacheViewModel
                            {
                                Description = "Élaboration d'un plan d'évolution des infrastructures technologiques"
                            }
                        },
                        Technologies =
                        {
                            new TechnologieViewModel
                            {
                                Description = "Visual basic .net",
                                Mois = 15
                            },
                            new TechnologieViewModel
                            {
                                Description = "Azure",
                                Mois = 20
                            },
                            new TechnologieViewModel
                            {
                                Description = "Linux",
                                Mois = 15
                            }
                        }
                    }
                }
            };

            // Améliorer: obtenir-les du repositoire.
            ViewBag.ConseillerFonctions = new Dictionary<string, string>
            {
                { "Analyste Programeur", "Analyste Programeur"},
                { "Analyste devéloppeur .Net", "Analyste devéloppeur .Net"},
                { "Architect", "Architect"}
            };

            return model;
        }

        public CVPocController()
        {
            UtilisateurDepot = new UtilisateurGraphRepository();
        }

        // GET: CV
        public ActionResult Index()
        {
            return View();
        }

        // GET: CV/Details/5
        [Route("Details/{cvId}")]
        [AllowAnonymous]
        public ActionResult Details(string id)
        {
            //return Json(CreateDummyCVViewModel());

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(null);
            }

            // var cVViewModel = CreateDummyCVViewModel();
            Utilisateur utilisateur = UtilisateurDepot.GetOne(id);
            if(utilisateur == null)
            {
                return Json(null);
            }

            CVMapper mapper = new CVMapper();
            CVViewModel cVViewModel = mapper.Map(utilisateur);
            return Json(cVViewModel);
        }

        // POST: CV/Edit/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Edit")]
        public ActionResult Edit([FromBody]CVViewModel cv)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "CV modifiée" });
        }
    }
}
