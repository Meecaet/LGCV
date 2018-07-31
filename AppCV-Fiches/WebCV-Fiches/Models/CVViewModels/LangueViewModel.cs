using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class LangueViewModel : ViewModel
    {
        public string Nom { get; set; }
        public string NiveauParle { get; set; }
        public string NiveauEcrit { get; set; }
        public string NiveauLu { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            if(edition.ProprieteNom == "Langues")
                return true;

            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ProprieteNom);
        }
    }
}
