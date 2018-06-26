using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Controllers
{
    public class CVController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private ApplicationUser UtilisateurActuel;
        private UtilisateurGraphRepository UtilisateurDepot;

        public CVController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        // GET: CV
        public async Task<ActionResult> Index()
        {
            UtilisateurDepot = new UtilisateurGraphRepository("Graph_CV", "CVs");
            UtilisateurActuel = await userManager.GetUserAsync(User);

            var utilisateur = UtilisateurDepot.Search(new Utilisateur { NomComplet = UtilisateurActuel.NomComplet }).DefaultIfEmpty(null).FirstOrDefault();

            if (utilisateur != null)
                return RedirectToAction(nameof(Details));
            else
                return RedirectToAction(nameof(Create));            
        }

        // GET: CV/Details/5
        public ActionResult Details(int id)
        {
            return View();
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