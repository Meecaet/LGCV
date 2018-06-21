using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlEntities
{
    public class XmlDocTable : XmlDocNode
    {        
        public XmlDocTable(XmlNode xmlNode) : base(xmlNode)
        {        }

        public List<XmlDocParagraph> GetParagraphsFromColumn(int column)
        {
            List<XmlDocParagraph> listOfParagraphs = new List<XmlDocParagraph>();
            var paragraphs = this.xmlNode.SelectNodes($".//w:tr/w:tc[{column}]/w:p", namespaceManager);

            foreach (XmlNode node in paragraphs)
                listOfParagraphs.Add(new XmlDocParagraph(node));

            return listOfParagraphs;
        }

        public List<XmlDocParagraph> GetParagraphsFromColumns()
        {
            List<XmlDocParagraph> listOfParagraphs = new List<XmlDocParagraph>();
            var paragraphs = this.xmlNode.SelectNodes($".//w:tr/w:tc/w:p", namespaceManager);

            foreach (XmlNode node in paragraphs)
                listOfParagraphs.Add(new XmlDocParagraph(node));

            return listOfParagraphs;
        }

        public int CountColumns()
        {
            List<XmlNode> listOfNodes = new List<XmlNode>();
            var nodes = this.xmlNode.SelectNodes($".//w:tr[1]/w:tc", namespaceManager);           
            return nodes.Count;
        }        

        public List<string> GetAllLines()
        {
            List<string> listLines = new List<string>();
            var paragraphs = this.xmlNode.SelectNodes($".//w:tr", namespaceManager);

            string currentLine = string.Empty;
            foreach (XmlNode trNode in paragraphs)
            {
                foreach (XmlNode prNode in trNode.SelectNodes(".//w:p", namespaceManager))
                {
                    currentLine += prNode.InnerText;
                    currentLine += " ";
                }

                listLines.Add(currentLine.Trim());
                currentLine = string.Empty;
            }

            return listLines;
        }
    }
}
