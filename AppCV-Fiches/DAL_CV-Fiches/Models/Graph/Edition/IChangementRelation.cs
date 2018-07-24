using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public interface IChangementRelation : IEditionObject
    {
        string ObjetAjouteId { get; set; }
        string ObjetSupprimeId { get; set; }
    }
}
