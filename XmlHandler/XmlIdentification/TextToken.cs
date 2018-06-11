using XmlHandler.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
{
    public class TextToken : IXmlToken
    {

        private Dictionary<string, string[]> textTokens;
        

        public TextToken()
        {
            textTokens = new Dictionary<string, string[]>
            {
                { "PRINCIPAUX DOMAINES D’INTERVENTION", new string[] { "PRINCIPAUX DOMAINES D’INTERVENTION" } },
                { "FORMATION ACADÉMIQUE", new string[] { "FORMATION ACADÉMIQUE" } },
                { "RÉSUMÉ DES INTERVENTIONS", new string[] { "RÉSUMÉ DES INTERVENTIONS" } },
                { "PERFECTIONNEMENT", new string[] { "PERFECTIONNEMENT" } },
                { "CONFÉRENCES", new string[] { "CONFÉRENCES", "AUTRES FORMATIONS" } },
                { "PUBLICATIONS", new string[] { "PUBLICATIONS" } },
                { "ASSOCIATIONS", new string[] { "ASSOCIATIONS" } },
                { "LANGUES PARLÉES, ÉCRITES", new string[] { "LANGUES PARLÉES, ÉCRITES" } }
            };
        }

        public bool Match(XmlNode node, out string identificant)
        {
            string innerText;
            identificant = string.Empty;

            if (string.IsNullOrEmpty(node.InnerText))
                return false;

            if (node.Name == "w:p")
            {
                innerText = node.InnerText.Trim();
                foreach (KeyValuePair<string, string[]> token in textTokens)
                {
                    if (token.Value.Any(x => innerText.ToUpper().Equals(x)))
                    {
                        identificant = token.Key;
                        return true;
                    }
                }
            }
            else
            {
                XmlDocTable table = new XmlDocTable(node);
                List<XmlDocParagraph> paragraphs = table.GetParagraphsFromColumns();

                foreach (XmlDocParagraph paragraph in paragraphs)
                {
                    innerText = paragraph.GetParagraphText().Trim();
                    foreach (KeyValuePair<string, string[]> token in textTokens)
                    {
                        if (token.Value.Any(x => innerText.ToUpper().Equals(x)))
                        {
                            identificant = token.Key;
                            return true;
                        }
                    }
                }
            }           
          
            return false;
        }
    }
}
