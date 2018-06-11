using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Certification
    {
        public string Description { get; set; }

        private Certification()
        { }

        public static Certification CreateCertification(string certification)
        {
            Certification cert = new Certification();
            cert.Description = certification;

            return cert;
        }
    }
}
