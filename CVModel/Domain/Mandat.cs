using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    public class Mandat
    {
        public int Numero { get; set; }
        public string Projet { get; set; }
        public int Envenrgure { get; set; }
        public string Fonction { get; set; }
        public string Periode { get; set; }
        public string Efforts { get; set; }
        public string Reference { get; set; }
    }
}
