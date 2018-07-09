using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Langue")]
    public class CVLangueController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new CVViewModel() { GraphIdCV = cvId });
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(string cvId, string langueID)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "langue ajouté." });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Delete(string cvId, string langueId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Langue eliminé" });
        }
    }
}