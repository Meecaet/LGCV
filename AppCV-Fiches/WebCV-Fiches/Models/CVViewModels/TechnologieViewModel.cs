using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class TechnologieViewModel : ViewModel
    {
        public string GraphId { get; set; }
        public string Description { get; set; }
        public double Mois { get; set; }
        public List<EditionObjecViewModel> editionObjecViewModels { get; set; }
    }
}
