using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("ResumeIntervention")]
    public class CVResumeInterventionsController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string utilisateurId)
        {

        
            return Json(new List<ResumeInterventionViewModel> {
                new ResumeInterventionViewModel{
                    Annee = 2007,
                    Client = "Colmayor",
                    Effors = 1200,
                    Envergure = 3000,
                    Fonction = "Analyste programmeur",
                    MandatId = "2a5110b1-bb07-4981-9a3d-33aaedde49f0",
                    Nombre = 1,
                    Projet = "Amélioration du module de projets"
                },
                new ResumeInterventionViewModel{
                    Annee = 2010,
                    Client = "Thomas Greg",
                    Effors = 1200,
                    Envergure = 8000,
                    Fonction = "Analyste programmeur .Net",
                    MandatId = "13d8799f-ebb2-4501-b3a6-2053c0fd7ea5",
                    Nombre = 2,
                    Projet = "Développement d’un module d’indicateurs de gestion"
                },
                new ResumeInterventionViewModel{
                    Annee = 2011,
                    Client = "Processa",
                    Effors = 1100,
                    Envergure = 8450,
                    Fonction = "Analyste programmeur .Net",
                    MandatId = "c725b9f0-7dfe-4ce0-a9a4-2448707197e7",
                    Nombre = 3,
                    Projet = "Amélioration d’un système SIG"
                }
            });
        }
    }
}