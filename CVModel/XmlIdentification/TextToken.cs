using CVModel.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlIdentification
{
    public class TextToken : IXmlToken
    {
        private List<string> textTokens;

        public TextToken()
        {
            textTokens = new List<string>()
            {
                "PRINCIPAUX DOMAINES D’INTERVENTION",
                "FORMATION ACADÉMIQUE",
                "RÉSUMÉ DES INTERVENTIONS",
                "PERFECTIONNEMENT",
                "CONFÉRENCES SUIVIES",
                "LANGUES PARLÉES, ÉCRITES"
            };
        }

        public bool Match(XmlNode node, out string identificant)
        {
            string innerText;
            identificant = string.Empty;

            if (string.IsNullOrEmpty(node.InnerText))
                return false;

            innerText = node.InnerText.Trim();
            foreach (string token in textTokens)
            {
                if (innerText.ToUpper().Contains(token))
                {
                    identificant = token;
                    return true;
                }
            }

            if (textTokens.Contains(innerText.ToUpper()))
                return true;

            return false;
        }
    }
}
