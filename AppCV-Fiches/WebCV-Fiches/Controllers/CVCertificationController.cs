using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Certification")]
    public class CVCertificationController : Controller
    {
        [Route("{cvId}/All")]
        [AllowAnonymous]
        public ActionResult All(string cvId)
        {
            return Json(new List<CertificationViewModel> { });
        }

        [Route("Detail/{certificationId}")]
        [AllowAnonymous]
        public ActionResult Detail(string cvId, string certificationId)
        {
            return Json(new CertificationViewModel());
        }

        // POST: Mandat/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Add")]
        public ActionResult Add(string cvId, [FromBody]CertificationViewModel certification)
        {
            // peut être le nouveau graphId.
            return Json(certification);
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{cvId}/Edit")]
        public ActionResult Edit(string cvId, [FromBody]CertificationViewModel certification)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Certification modifiée" });
        }

        // POST: Mandat/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [Route("Delete")]
        public ActionResult Delete(string cvId, string certificationId)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Certification eliminé" });
        }
    }
}