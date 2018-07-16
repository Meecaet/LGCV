using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Models.Rapport
{
    public class FormationAcademique
    {
        public int Id { get; set; }
        public string Diplome { get; set; }
        public int AnDuTitre { get; set; }
        public string Instituition { get; set; }       
    }
}
