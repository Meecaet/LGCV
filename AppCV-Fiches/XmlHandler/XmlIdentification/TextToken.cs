using XmlHandler.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
{
    /// <summary>
    /// Cette IXmlToken vérifie le text d'un paragraphe et, s'il est exactement le même qu'une string dans une liste, ça retourne un Match vrai
    /// </summary>
    public class TextToken : IXmlToken
    {
        protected Dictionary<string, string[]> textTokens;

        protected TextToken()
        {
            //Le dictionaire contient l'identifiant et la liste des textes qui seront utilisés pour la vérification. Cette liste ici c'est la liste standard
            textTokens = new Dictionary<string, string[]>
            {
                { "PRINCIPAUX DOMAINES D’INTERVENTION", new string[] { "PRINCIPAUX DOMAINES D’INTERVENTION" } },
                { "FORMATION ACADÉMIQUE", new string[] { "FORMATION ACADÉMIQUE" } },
                { "RÉSUMÉ DES INTERVENTIONS", new string[] { "RÉSUMÉ DES INTERVENTIONS" } },
                { "PERFECTIONNEMENT", new string[] { "PERFECTIONNEMENT" } },
                { "CONFÉRENCES", new string[] { "CONFÉRENCES", "AUTRES FORMATIONS" } },
                { "PUBLICATIONS", new string[] { "PUBLICATIONS" } },
                { "ASSOCIATIONS", new string[] { "ASSOCIATIONS" } },
                { "TECHNOLOGIES", new string[] { "TECHNOLOGIES" } },
                { "LANGUES PARLÉES, ÉCRITES", new string[] { "LANGUES PARLÉES, ÉCRITES" } }
            };
        }

        public static TextToken CreateTextToken()
        {
            return new TextToken();
        }

        protected virtual Dictionary<string, string[]> GetTokens()
        {
            return textTokens;
        }

        protected virtual bool Comparaison(KeyValuePair<string, string[]> token, string innerText)
        {
            return token.Value.Any(x => innerText.ToUpper().Equals(x));
        }

        public bool Match(XmlNode node, out string identificant)
        {
            string innerText;
            identificant = string.Empty;

            if (string.IsNullOrEmpty(node.InnerText))
                return false;

            //Vérifie le texte dans un paragraphe
            if (node.Name == "w:p")
            {
                innerText = node.InnerText.Trim();
                foreach (KeyValuePair<string, string[]> token in GetTokens())
                {
                    if (Comparaison(token, innerText))
                    {
                        identificant = token.Key;
                        return true;
                    }
                }
            }
            else //Ou dans le paragraphes dans une tableau
            {
                XmlDocTable table = new XmlDocTable(node);
                List<XmlDocParagraph> paragraphs = table.GetParagraphsFromColumns();

                foreach (XmlDocParagraph paragraph in paragraphs)
                {
                    innerText = paragraph.GetParagraphText().Trim();
                    foreach (KeyValuePair<string, string[]> token in GetTokens())
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
