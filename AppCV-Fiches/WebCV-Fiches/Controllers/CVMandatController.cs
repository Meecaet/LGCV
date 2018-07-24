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
    [Route("Mandat")]
    public class CVMandatController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new List<MandatViewModel>() { });
        }

        [Route("{cvId}/Detail/{mandatId}")]
        [AllowAnonymous]
        public ActionResult Detail(string cvId, string mandatId)
        {
            return Json(new MandatViewModel());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Add")]
        public ActionResult Add(string cvId, [FromBody]MandatViewModel mandat)
        {
            // peut être le nouveau graphId.
            return Json(mandat);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Edit")]
        public ActionResult Edit(string cvId, [FromBody]MandatViewModel mandat)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status="OK", Message="Mandat modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string mandatId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Mandat eliminé" });
        }
    }
}