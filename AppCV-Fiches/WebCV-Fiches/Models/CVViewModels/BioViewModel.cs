using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class BioViewModel : ViewModel
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string ResumeExperience { get; set; }
        public string Fonction { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ProprieteNom);
        }
    }
}
