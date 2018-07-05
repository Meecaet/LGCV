using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Graph
{
    public class PieceJointe : GraphObject
    {
        public string NomduFichier { get; set; }

        [Edge("DuType")]
        public Genre Type { get; set; }

        public string MimeType { get; set; }
        public string CheminDuFichier { get; set; }
        public string Commentaire { get; set; }
        public DateTime Date { get; set; }
    }
}
