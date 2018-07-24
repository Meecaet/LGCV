using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class LangueViewModel : ViewModel
    {
        public string GraphId { get; set; }
        public string Nom { get; set; }
        public string NiveauParle { get; set; }
        public string NiveauEcrit { get; set; }
        public string NiveauLu { get; set; }
    }
}
