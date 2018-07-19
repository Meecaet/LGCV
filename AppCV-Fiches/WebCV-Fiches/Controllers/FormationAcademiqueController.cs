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
    [Route("FormationAcademique")]
    public class FormationAcademiqueController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new List<FormationAcademiqueViewModel> { } );
        }

        [Route("/Detail/{utilisatuerId}")]
        [AllowAnonymous]
        public ActionResult Detail(string utilisatuerId)
        {
            return Json(new FormationAcademiqueViewModel());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Add")]
        public ActionResult Add(string cvId, [FromBody]FormationAcademiqueViewModel formationAcademique)
        {
            // peut être le nouveau graphId.
            return Json(formationAcademique);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Edit")]
        public ActionResult Edit(string cvId, [FromBody]FormationAcademiqueViewModel formationAcademique)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status="OK", Message="Formation académique modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string formationAcademiqueId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Formation académique eliminé" });
        }
    }
}