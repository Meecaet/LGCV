using DAL_CV_Fiches.Models.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class BioViewModel : ViewModel
    {
        [JsonProperty("nom")]
        public string Nom { get; set; }
        [JsonProperty("prenom")]
        public string Prenom { get; set; }
        [JsonProperty("resumeExperience")]
        public string ResumeExperience { get; set; }
        [JsonProperty("fonction")]
        public string Fonction { get; set; }

        public override bool HasEdtion(EditionObject edition)
        {
            var propreties = MethodBase.GetCurrentMethod().DeclaringType.GetProperties().ToList();
            return propreties.Any(x => x.Name == edition.ProprieteNom);
        }
    }
}
