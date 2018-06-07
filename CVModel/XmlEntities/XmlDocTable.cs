using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlEntities
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
    }
}
