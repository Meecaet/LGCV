using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class MandatViewModel : ViewModel
    {
        public string GraphIdProjet { get; set; }
        public string GraphIdClient { get; set; }
        public string GraphIdFonction { get; set; }
        public string GraphIdSocieteDeConseil { get; set; }

        public string NomClient { get; set; }
        public int NumeroMandat { get; set; }
        public string NomEntreprise { get; set; }
        public string TitreProjet { get; set; }
        public string TitreMandat { get; set; }
        public int Envergure { get; set; }
        public int Efforts { get; set; }
        public string Fonction { get; set; }
        public string ContexteProjet { get; set; }
        public string PorteeDesTravaux { get; set; }
        public List<TacheViewModel> Taches { get; set; }
        public List<TechnologieViewModel> Technologies { get; set; }


        //Date projet 
        public DateTime DebutProjet { get; set; }
        public DateTime FinProjet { get; set; }
        // Date Mandat
        public DateTime DebutMandat { get; set; }
        public DateTime FinMandat { get; set; }


        public string NomReference { get; set; }
        public string FonctionReference { get; set; }
        public string TelephoneReference { get; set; }
        public string CellulaireReference { get; set; }
        public string CourrielReference { get; set; }

        public MandatViewModel()
        {
            Taches = new List<TacheViewModel>();
            Technologies = new List<TechnologieViewModel>();
        }

        public override bool HasEdtion(EditionObject edition)
        {
            if (edition.ViewModelProprieteNom == "Mandats")
                return true;

            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ViewModelProprieteNom);
        }
    }
}
