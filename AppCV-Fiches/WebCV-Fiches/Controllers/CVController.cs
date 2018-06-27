using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Controllers
{
    public class CVController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private ApplicationUser UtilisateurActuel;
        private UtilisateurGraphRepository UtilisateurDepot;
        private object mapper;

        public CVController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            UtilisateurDepot = new UtilisateurGraphRepository("Graph_CV", "CVs");
        }

        // GET: CV
        public async Task<ActionResult> Index()
        {
            UtilisateurActuel = await userManager.GetUserAsync(User);

            Utilisateur utilisateur;

            if (HttpContext.Session.Get<Utilisateur>("Utilisateur") != null)
                utilisateur = HttpContext.Session.Get<Utilisateur>("Utilisateur");
            else
            {
                utilisateur = UtilisateurDepot.Search(new Utilisateur { Nom = UtilisateurActuel.NomComplet }).DefaultIfEmpty(null).FirstOrDefault();
                HttpContext.Session.Set<Utilisateur>("Utilisateur", utilisateur);
            }


            if (utilisateur != null)
                return RedirectToAction(nameof(Details), new { id = utilisateur.GraphKey });
            else
                return RedirectToAction(nameof(Create));
        }

        // GET: CV/Details/5
        public ActionResult Details(string id)
        {
            Utilisateur utilisateur;

            if (HttpContext.Session.Get<Utilisateur>("Utilisateur") != null)
                utilisateur = HttpContext.Session.Get<Utilisateur>("Utilisateur");
            else
            {
                utilisateur = UtilisateurDepot.GetOne(id);
                HttpContext.Session.Set<Utilisateur>("Utilisateur", utilisateur);
            }

            CVMapper mapper = new CVMapper();
            CVViewModel cVViewModel = mapper.Map(utilisateur);
            return View(cVViewModel);
        }

        // GET: CV/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CV/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CV/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CV/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CV/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CV/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}