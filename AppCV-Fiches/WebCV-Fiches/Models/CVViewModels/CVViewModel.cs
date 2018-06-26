using DAL_CV_Fiches.Models;
using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class CVViewModel
    {
        public string GraphIdConseiller { get; set; }
        public string GraphIdUtilisateur { get; set; }
        public string GraphIdFonction { get; set; }
        public string GraphIdCV { get; set; }

        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Fonction { get; set; }
        public string Biographie { get; set; }
        public List<DomaineDInterventionViewModel> DomainesDIntervention { get; set; }
        public List<FormationAcademiqueViewModel> FormationsAcademique { get; set; }
        public List<CertificationViewModel> Certifications { get; set; }
        public List<MandatViewModel> Mandats { get; set; }
        public List<TechnologieViewModel> Technologies { get; set; }
        public List<PerfectionnementViewModel> Perfectionnements { get; set; }
        public List<LangueViewModel> Langues { get; set; }

        public CVViewModel()
        {
            DomainesDIntervention = new List<DomaineDInterventionViewModel>();
            FormationsAcademique = new List<FormationAcademiqueViewModel>();
            Certifications = new List<CertificationViewModel>();
            Mandats = new List<MandatViewModel>();
            Technologies = new List<TechnologieViewModel>();
            Perfectionnements = new List<PerfectionnementViewModel>();
            Langues = new List<LangueViewModel>();
        }
    }
    
}
