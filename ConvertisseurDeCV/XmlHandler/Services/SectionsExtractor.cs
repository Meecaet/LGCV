using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlHandler.XmlEntities;
using XmlHandler.XmlIdentification;

namespace XmlHandler.Services
{
    public class SectionsExtractor
    {
        public List<XmlNode> GetCVNodes(string xmlFilePath, bool keepWhiteSpaces = false)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(xmlFilePath);

            XmlNode rootNnode = doc.LastChild.LastChild;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode item in rootNnode.ChildNodes)
            {
                if (!keepWhiteSpaces)
                {
                    if (item.Name == "w:p" && string.IsNullOrEmpty(item.InnerText))
                        continue;

                    //TODO: Ajouter la rémotion des paragraphs vides

                    //if (item.Name == "w:tbl")
                    //{
                    //    foreach (XmlNode internalNode in item.ChildNodes)
                    //    {
                    //        if (internalNode.Name == "w:p" && string.IsNullOrEmpty(internalNode.InnerText))
                    //            item.RemoveChild(internalNode);

                    //    }
                    //}
                }                

                nodes.Add(item);
            }

            return nodes;
        }

        public List<CVSection> GetCVSections(List<XmlNode> Nodes, List<IXmlToken> matchTokens, string firstIdentifier, bool skipFirstNode = false)
        {
            string currentIdent = string.Empty;

            XmlNode First;
            List<CVSection> Sections = new List<CVSection>();                        
            CVSection currentCVSection = new CVSection();
            currentCVSection.Identifiant = firstIdentifier;

            if (skipFirstNode)
            {
                First = Nodes.First();
                Nodes.Remove(First);

                currentCVSection.AddNode(First);
            }

            while (Nodes.Count > 0)
            {
                First = Nodes.First();
                Nodes.Remove(First);

                if (matchTokens.Any(x => x.Match(First, out currentIdent)))
                {
                    Sections.Add(currentCVSection);

                    currentCVSection = new CVSection();
                    currentCVSection.Identifiant = currentIdent;
                    currentCVSection.AddNode(First);

                    currentIdent = string.Empty;
                }
                else
                {
                    currentCVSection.AddNode(First);
                }
            }

            Sections.Add(currentCVSection);

            return Sections;
        }
    }
}
