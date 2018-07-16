using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Rapport
{
    public class CV
    {
        public int Id { get; set; }
        public string Nom{ get; set; }
        public string Fonction { get; set; }
        public string Bio { get; set; }
        
        public List<DomaineDIntervention> DomaineDInterventions { get; set; }
        public List<FormationAcademique> FormationAcademiques { get; set; }
        public List<Certification> Certifications { get; set; }
        public List<Employeur> Employeurs { get; set; }
        public List<TechnologieCV> TechnologiesCV { get; set; }
        public List<Perfectionnement> Perfectionnements { get; set; }
        public List<Associations> Associations { get; set; }
        public List<Publications> Publications { get; set; }
        public List<Conferences> Conferences { get; set; }
        public List<Langues> Langues { get; set; }
        public List<ImagesAttachees> ImagesAttachees { get; set; }        
    }
}
