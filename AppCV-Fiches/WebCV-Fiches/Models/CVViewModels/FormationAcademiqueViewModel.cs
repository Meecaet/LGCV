using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class FormationAcademiqueViewModel : ViewModel
    {
        public string GraphIdEtablissement { get; set; }

        public string Diplome { get; set; }
        public int Annee { get; set; }
        public string Etablissement { get; set; }
        public string Pays { get; set; }
        public int Niveau { get; set; }
        public bool Principal{ get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            if(edition.ProprieteNom == "FormationsScolaires")
                return true;

            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ProprieteNom);
        }
    }
}
