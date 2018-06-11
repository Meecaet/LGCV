using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Client
    {
        public string Nom { get; set; }
        public List<Mandat> Mandats { get; set; }

        public Client()
        {
            Mandats = new List<Mandat>();
        }        
    }
}
