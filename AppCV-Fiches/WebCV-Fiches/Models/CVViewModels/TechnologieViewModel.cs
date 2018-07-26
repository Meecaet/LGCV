using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class TechnologieViewModel : ViewModel
    {
        public string Description { get; set; }
        public double Mois { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            if(edition.ProprieteNom == "Technologies")
                return true;

            return false;
        }
    }
}
