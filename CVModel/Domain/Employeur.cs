using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    public class Employeur
    {
        public string Nom { get; set; }
        public List<Client> Clients { get; set; }
    }
}
