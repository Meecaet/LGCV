using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public class Certification : GraphObject
    {
        public string Description { get; set; }

        public Certification()
        { }

        public static Certification CreateCertification(string certification)
        {
            Certification cert = new Certification();
            cert.Description = certification;

            return cert;
        }
    }
}
