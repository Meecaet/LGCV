using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models
{
    public class ApiCredential
    {
        public bool authenticated { get; set; }
        public string created { get; set; }
        public string expiration { get; set; }
        public string Token { get; set; }
        public string message { get; set; }
        public string utilisateurId { get; set; }
    }
}
