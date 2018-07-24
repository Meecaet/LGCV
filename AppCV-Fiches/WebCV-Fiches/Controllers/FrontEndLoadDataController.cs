using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    [Authorize("Bearer")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class FrontEndLoadDataController : Controller
    {
        private IMemoryCache cache;
        public FrontEndLoadDataController(IMemoryCache cache)
        {

            this.cache = cache;
        }
        [HttpGet]
        public ActionResult GetAllLangues()

        {
            List<LangueViewModel> langues = new List<LangueViewModel>();
            var lang = (List<Langue>)cache.Get("Langues");
            if (lang != null)
            {
                lang.OrderBy(o => o.Nom).ToList().ForEach(x =>
             {
                 langues.Add(new LangueViewModel { GraphId = x.GraphKey, Nom = x.Nom });
             });
            }
            return Json(langues);
        }
        [HttpGet]
        public ActionResult GetAllTechnologies()
        {
            List<TechnologieViewModel> langues = new List<TechnologieViewModel>();
            var lang = (List<Technologie>)cache.Get("Technologies");
            if (lang != null)
            {
                lang.OrderBy(o => o.Nom).ToList().ForEach(x =>
            {
                langues.Add(new TechnologieViewModel { GraphId = x.GraphKey, Description = x.Nom });
            });
            }
            return Json(langues);
        }
        [HttpGet]
        public ActionResult GetAllFonctions()
            {
            List<FonctionViewModel> langues = new List<FonctionViewModel>();
            var lang = (List<Fonction>)cache.Get("Fonction");
            if (lang != null)
            {
                lang.OrderBy(o => o.Description).ToList().ForEach(x =>
                {
                    langues.Add(new FonctionViewModel { GraphId = x.GraphKey, Nom = x.Description });
                });
            }

            return Json(langues);
        }
    }
}