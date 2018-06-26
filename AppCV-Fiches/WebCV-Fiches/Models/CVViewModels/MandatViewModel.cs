using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class MandatViewModel
    {
        public string GraphId { get; set; }

        public string NomClient { get; set; }
        public int NumeroMandat { get; set; }
        public string NomEntreprise { get; set; }
        public string TitreProjet { get; set; }
        public string TitreMandat { get; set; }
        public string Envergure { get; set; }
        public string Efforts { get; set; }
        public string Fonction { get; set; }
        public string ContexteProjet { get; set; }
        public string PorteeDesTravaux { get; set; }
        public List<TacheViewModel> Taches { get; set; }
        public List<TechnologieViewModel> Technologies { get; set; }
        public DateTime DebutProjet { get; set; }
        public DateTime FinProjet { get; set; }
        public DateTime DebutMandat { get; set; }
        public DateTime FinMandat { get; set; }

        public string NomReference { get; set; }
        public string FonctionReference { get; set; }
        public string TelephoneReference { get; set; }
        public string CelluaireReference { get; set; }
        public string CourrielReference { get; set; }
    }
}
