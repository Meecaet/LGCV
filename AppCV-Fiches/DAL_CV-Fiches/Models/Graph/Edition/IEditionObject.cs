using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public interface IEditionObject
    {
        string Etat { get; set; }
        string Observacao { get; set; }
        string Type { get; set; }
        GraphObject NoeudModifie { get; set; }
    }
}
