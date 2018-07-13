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
                    MandatId = Guid.NewGuid().ToString(),
                    Nombre = 1,
                    Projet = "Système académique et de géstion"
                },
                new ResumeInterventionViewModel{
                    Annee = 2010,
                    Client = "Thomas Greg",
                    Effors = 1200,
                    Envergure = 8000,
                    Fonction = "Analyste programmeur .Net",
                    MandatId = Guid.NewGuid().ToString(),
                    Nombre = 2,
                    Projet = "Système de gestion d'inventaires"
                },
                new ResumeInterventionViewModel{
                    Annee = 2011,
                    Client = "Processa",
                    Effors = 1100,
                    Envergure = 8450,
                    Fonction = "Analyste programmeur .Net",
                    MandatId = Guid.NewGuid().ToString(),
                    Nombre = 3,
                    Projet = "Interactive Voice Response"
                }
            });
        }
    }
}