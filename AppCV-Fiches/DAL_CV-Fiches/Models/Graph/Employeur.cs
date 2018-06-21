using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    public class Employeur : GraphObject
    {
        public string Nom { get; set; }

        [EdgeProperty]
        public DateTime DateDebut { get; set; }
        [EdgeProperty]
        public DateTime DateFin { get; set; }
        [EdgeProperty]
        public string DescriptionDuTravail { get; set; }
    }
}
