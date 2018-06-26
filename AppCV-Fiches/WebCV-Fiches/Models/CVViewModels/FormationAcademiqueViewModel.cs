using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class FormationAcademiqueViewModel
    {
        public string GraphId { get; set; }
        public string GraphIdEtablissement { get; set; }

        public string Diplome { get; set; }
        public int Annee { get; set; }
        public string Etablissement { get; set; }
    }
}
