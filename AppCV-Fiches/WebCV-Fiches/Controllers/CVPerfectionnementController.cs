using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Perfectionnement")]
    public class CVPerfectionnementController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new List<PerfectionnementViewModel> { });
        }

        [Route("/Detail/{utlisateurId}")]
        [AllowAnonymous]
        public ActionResult Detail(string utlisateurId)
        {
            return Json(new PerfectionnementViewModel());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Add")]
        public ActionResult Add(string cvId, [FromBody]PerfectionnementViewModel perfectionnement)
        {
            // peut être le nouveau graphId.
            return Json(perfectionnement);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Edit")]
        public ActionResult Edit(string cvId, [FromBody]PerfectionnementViewModel perfectionnement)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Perfectionnement modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string perfectionnementId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Perfectionnement eliminé" });
        }
    }
}