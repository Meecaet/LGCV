using CVModel.XmlEntities;
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
        public string Periode { get; set; }
        public List<Client> Clients { get; set; }

        public Employeur()
        {
            Clients = new List<Client>();
        }

        internal void AssemblerEmployeur(CVSection employeurSection)
        {
            XmlDocParagraph emplDesc = (XmlDocParagraph)employeurSection.Nodes.First(x => x is XmlDocParagraph);
            string[] info = emplDesc.GetLinesWithTab();

            Periode = info[0];
            Nom = info[1];
        }

    }
}
