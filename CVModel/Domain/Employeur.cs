using CVModel.XmlEntities;
using CVModel.XmlIdentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
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

            AssemblerClients(employeurSection);
        }

        internal void AssemblerClients(CVSection employeurSection)
        {
            IXmlToken styleToken = new FormatationToken(employeurSection.Nodes.First().OriginalNode.OwnerDocument.NameTable);
            ((FormatationToken)styleToken).SetStyleParameter(new KeyValuePair<string, string>("w:val", "Titre2"));

            List<CVSection> clientSections = new List<CVSection>();
            List<XmlDocNode> empNodes = new List<XmlDocNode>();

            CVSection clientSection = new CVSection();
            XmlDocNode first;
            string identifiant = string.Empty;

            clientSection.Identifiant = "Titre2";

            empNodes = employeurSection.Nodes.Skip(1).ToList();
            while (empNodes.Count > 0)
            {
                first = empNodes.First();
                empNodes.Remove(first);

                if (styleToken.Match(first.OriginalNode, out identifiant))
                {
                    if(clientSection.Nodes.Count > 0)
                        clientSections.Add(clientSection);

                    clientSection = new CVSection();
                    clientSection.Identifiant = identifiant;

                    identifiant = string.Empty;
                }

                clientSection.AddNode(first.OriginalNode);
            }

            clientSections.Add(clientSection);

            clientSections.ForEach(x => 
            {
                Client client = new Client();
                client.AssemblerClient(x);

                Clients.Add(client);
            });
        }

    }
}
