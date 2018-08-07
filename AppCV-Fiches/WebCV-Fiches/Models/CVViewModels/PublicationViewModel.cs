using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class PublicationViewModel : ViewModel
    {
        public string GraphIdGenre { get; set; }
        public string Description { get; set; }
        public int Annee { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            if(edition.ViewModelProprieteNom == "Formations")
                return true;

            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ViewModelProprieteNom);
        }
    }
}
