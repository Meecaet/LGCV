using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    public class CVTechnologiesController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new List<TechnologieViewModel> { });
        }

        [Route("{cvId}/Detail/{technologieId}")]
        [AllowAnonymous]
        public ActionResult Detail(string cvId, string technologieId)
        {
            return Json(new TechnologieViewModel());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Add")]
        public ActionResult Add(string cvId, [FromBody]TechnologieViewModel technologie)
        {
            // peut être le nouveau graphId.
            return Json(technologie);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Edit")]
        public ActionResult Edit(string cvId, [FromBody]TechnologieViewModel technologie)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "technologie modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string technologieId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "technologie eliminé" });
        }
    }
}