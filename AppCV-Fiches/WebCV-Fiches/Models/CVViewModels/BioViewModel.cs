using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class BioViewModel
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string ResumeExperience { get; set; }
        public string Fonction { get; set; }
        public string GraphIdConseiller { get; set; }
        public string GraphIdUtilisateur { get; set; }
        public string GraphIdFonction { get; set; }
        public string GraphIdCV { get; set; }
    }
}
