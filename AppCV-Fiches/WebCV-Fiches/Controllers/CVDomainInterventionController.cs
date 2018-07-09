using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("CVDomainIntervention")]
    public class CVDomainInterventionController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new CVDomainInterventionController());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("Add")]
        public ActionResult Add(string cvId, string domaineInterventionId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Domaine d'intervention ajouté." });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string domaineInterventionId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Domaine d'intervention eliminé" });
        }
    }
}