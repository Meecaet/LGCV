using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Route("Bio")]
    public class CVBioController : Controller
    {
        [Route("Detail/{utilisateurId}")]
        //[AllowAnonymous]
        [Authorize("Bearer")]
        public ActionResult Detail(string cvId, string utilisateurId)
        {
            return Json(new BioViewModel());
        }

        // POST: Mandat/Edit/cvId
        [HttpPost]
        [AllowAnonymous]
        [Route("{utilisateurId}/Edit")]
        public ActionResult Edit(string utilisateurId, [FromBody]BioViewModel bio)
        {
            // Objet sugeré comme viewModel.
            return Json(new { Status = "OK", Message = "Biographie modifiée" });
        }
        [Authorize("Bearer")]
        [HttpPost]
        [Route("Create")]
        public ActionResult Create([FromBody]BioViewModel bio)
        {
            // Objet sugeré comme viewModel.


            bio.GraphIdUtilisateur = "retorno ok";
            return Json(bio);
        }
    }
}