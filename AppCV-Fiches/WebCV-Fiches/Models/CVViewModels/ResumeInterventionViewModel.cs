using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class ResumeInterventionViewModel : ViewModel
    {
        public string Nombre { get; set; }
        public string Client { get; set; }
        public string Projet { get; set; }
        public int Envergure { get; set; }
        public string Fonction { get; set; }
        public string Annee { get; set; }
        public int Effors { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
 
            if(edition.ProprieteNom == "Mandats")
                return true;

 
            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ProprieteNom);
        }
    }
}
