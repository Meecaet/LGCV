using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Mandat
    {
        public string Numero { get; set; }
        public string Projet { get; set; }
        public string Envenrgure { get; set; }
        public string Fonction { get; set; }
        public string Periode { get; set; }
        public string Efforts { get; set; }
        public string Reference { get; set; }

        public string Description { get; set; }
       
    }
}
