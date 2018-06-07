using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlIdentification
{
    public class FormatationToken : IXmlToken
    {
        private Dictionary<string, string> styles;
        private XmlNamespaceManager namespaceManager;

        public FormatationToken(XmlNameTable nametable)
        {
            styles = new Dictionary<string, string>()
            {
                { "w:val", "Titre1" }
            };

            namespaceManager = new XmlNamespaceManager(nametable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        }

        public bool Match(XmlNode node, out string identificant)
        {
            identificant = string.Empty;
            XmlNodeList styleNodes = node.SelectNodes(".//w:pPr/w:pStyle", namespaceManager);
            foreach (XmlNode styleItem in styleNodes)
            {
                foreach (XmlAttribute attr in styleItem.Attributes)
                {
                    if (styles.ContainsKey(attr.Name) && styles[attr.Name] == attr.Value)
                    {
                        identificant = styles[attr.Name];
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
