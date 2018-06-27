using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    public class CVController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private ApplicationUser UtilisateurActuel;
        private UtilisateurGraphRepository UtilisateurDepot;
        private object mapper;

        public CVController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            UtilisateurDepot = new UtilisateurGraphRepository("Graph_CV", "CVs");
        }

        private CVViewModel CreateDummyCVViewModel()
        {
            var model = new CVViewModel()
            {
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
                        NumeroMandat = 12,
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

        // GET: CV
        public async Task<ActionResult> Index()
        {
            UtilisateurActuel = await userManager.GetUserAsync(User);

            Utilisateur utilisateur;

            if (HttpContext.Session.Get<Utilisateur>("Utilisateur") != null)
                utilisateur = HttpContext.Session.Get<Utilisateur>("Utilisateur");
            else
            {
                utilisateur = UtilisateurDepot.Search(new Utilisateur { Nom = UtilisateurActuel.NomComplet }).DefaultIfEmpty(null).FirstOrDefault();
                HttpContext.Session.Set<Utilisateur>("Utilisateur", utilisateur);
            }


            if (utilisateur != null)
                return RedirectToAction(nameof(Details), new { id = utilisateur.GraphKey });
            else
                return RedirectToAction(nameof(Create));
        }

        // GET: CV/Details/5
        public ActionResult Details(string id)
        {

            var cv = CreateDummyCVViewModel();
            return View(cv);

            /*Utilisateur utilisateur;

            if (HttpContext.Session.Get<Utilisateur>("Utilisateur") != null)
                utilisateur = HttpContext.Session.Get<Utilisateur>("Utilisateur");
            else
            {
                utilisateur = UtilisateurDepot.GetOne(id);
                HttpContext.Session.Set<Utilisateur>("Utilisateur", utilisateur);
            }

            CVMapper mapper = new CVMapper();
            CVViewModel cVViewModel = mapper.Map(utilisateur);
            return View(cVViewModel);*/
        }

        // GET: CV/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CV/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CV/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CV/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CV/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CV/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}