using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class ResumeInterventionViewModel
    {
        public string MandatId { get; set; }
        public int Nombre { get; set; }
        public string Client { get; set; }
        public string Projet { get; set; }
        public int Envergure { get; set; }
        public string Fonction { get; set; }
        public int Annee { get; set; }
        public int Effors { get; set; }
    }
}
