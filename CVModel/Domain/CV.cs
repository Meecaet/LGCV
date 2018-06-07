using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    public class CV
    {
        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }

        public List<string> DomaineDIntervention { get; set; }
        public FormationAcademique FormationAcademique { get; set; }
        public List<string> Certifications { get; set; }

        public List<Employeur> Employeurs { get; set; }
        public List<string> Perfectionnement { get; set; }
        public List<string> Conferences { get; set; }
        public List<Lange> Langes { get; set; }

    }
}
