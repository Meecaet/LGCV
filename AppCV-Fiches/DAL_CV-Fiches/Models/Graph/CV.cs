using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DAL_CV_Fiches.Models.Graph
{
    public class CV : GraphObject
    {
        public string ResumeExperience { get; set; }
        public DateTime DateCreation { get; set; }
        public int Version { get; set; }
        public StatusCV Status { get; set; }
        public bool EstPricipal { get; set; }

        [EdgeProperty]
        public DateTime DateApprobation { get; set; }
    }

    public enum StatusCV
    {
        Approuve = 0,
        EnAttenteDApprobation = 1,
        EnRevision = 2,
        Nouveau = 3
    }
}
