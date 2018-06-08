using CVModel.XmlEntities;
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

        internal void AssemblerClient(CVSection clientSection)
        {
            XmlDocParagraph emplDesc = (XmlDocParagraph)clientSection.Nodes.DefaultIfEmpty(null).FirstOrDefault(x => x is XmlDocParagraph);

            if (emplDesc != null)
            {
                string[] info = emplDesc.GetLinesWithTab();
                Nom = string.Join(" ", info);
            }
            
            AssemblerMandats(clientSection);
        }

        internal void AssemblerMandats(CVSection clientSection)
        {
            List<XmlDocNode> mandatsNodes = new List<XmlDocNode>(),
            mandatNodes = new List<XmlDocNode>();

            mandatsNodes.AddRange(clientSection.Nodes.SkipWhile(x => x is XmlDocParagraph));

            do
            {
                mandatNodes.Add(mandatsNodes.First(x => x is XmlDocTable));
                mandatNodes.AddRange(mandatsNodes.SkipWhile(x => x is XmlDocTable).TakeWhile(x => x is XmlDocParagraph));

                mandatsNodes.RemoveAll(x => mandatNodes.Contains(x));
                Mandat mandat = new Mandat();
                mandat.AssemblerMandat(mandatNodes);
                Mandats.Add(mandat);

                mandatNodes.Clear();

            } while (mandatsNodes.Count > 0);
        }
    }
}
