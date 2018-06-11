using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
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

        public void SetStyleParameter(params KeyValuePair<string, string>[] stylesKeys)
        {
            styles.Clear();
            foreach (KeyValuePair<string, string> item in stylesKeys)
                styles.Add(item.Key, item.Value);
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
