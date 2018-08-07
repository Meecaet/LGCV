using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class DomaineDInterventionViewModel : ViewModel
    {
        public string Description { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            return edition.ViewModelProprieteNom == "DomaineDInterventions";
        }
    }
}
