using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WebCV_Fiches.Controllers
{
    [Route("api/CV")]
    public class CVController : Controller
    {
        public UtilisateurGraphRepository utilisateurGraphRepository;

        public CVController()
        {
            utilisateurGraphRepository = new UtilisateurGraphRepository();
        }

        [HttpPost]
        [Route("{utilisateurId}/EnvoyerPourApprobation")]
        public ActionResult EnvoyerPourApprobation(string utilisateurId)
        {
            var utilisateur = utilisateurGraphRepository.GetOne(utilisateurId);

            return Ok();
        }
    }
}