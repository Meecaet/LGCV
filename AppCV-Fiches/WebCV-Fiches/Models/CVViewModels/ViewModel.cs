using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public abstract class ViewModel
    {
        public string GraphId { get; set; }
        public List<EditionObjecViewModel> editionObjecViewModels { get; set; }

        public ViewModel()
        {
            editionObjecViewModels = new List<EditionObjecViewModel>();
        }

        public virtual bool HasEdtion(EditionObject edition)
        {
            throw new NotImplementedException();
        }
    }
}
