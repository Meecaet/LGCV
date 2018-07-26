using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class TacheViewModel : ViewModel
    {
        public string GraphId { get; set; }
        public string Description { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            if(edition.ProprieteNom == "Taches")
                return true;

            return false;
        }
    }
}
