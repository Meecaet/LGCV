using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
{
    /// <summary>
    /// Cette IXmlToken vérifie le format d'un paragraphe (taille du lettre, couler ou style) et, si la collection de clé et valuer trouvée dans le paragraphe est la même defini dans une liste, ça retourne un Match vrai
    /// </summary>
    public class FormatationToken : IXmlToken
    {
        private Dictionary<string, string> styles;
        private XmlNamespaceManager namespaceManager;

        private FormatationToken()
        {
            styles = new Dictionary<string, string>();
            namespaceManager = null;
        }       

        public static FormatationToken CreateFormatationToken(params KeyValuePair<string, string>[] stylesKeys)
        {
            FormatationToken formToken = new FormatationToken();
            formToken.SetStyleParameter(stylesKeys);

            return formToken;
        }

        //Cette XmlToken utilise des expressions XPath pour rechercher nœuds. Pour ça, on a besoin d'un XmlNameTable
        private void AddNamespaceManager(XmlNameTable nametable)
        {
            namespaceManager = new XmlNamespaceManager(nametable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        }

        public void SetStyleParameter(KeyValuePair<string, string>[] stylesKeys)
        {
            styles.Clear();
            foreach (KeyValuePair<string, string> item in stylesKeys)
                styles.Add(item.Key, item.Value);
        }

        public bool Match(XmlNode node, out string identificant)
        {
            identificant = string.Empty;

            if (namespaceManager == null)
                AddNamespaceManager(node.OwnerDocument.NameTable);

            //On a apperçu que le format est defini dans le nœud w:pStyle, dans le nœud w:pPr
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
