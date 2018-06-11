using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlEntities
{
    public class CVSection
    {
        public string Identifiant { get; set; }
        public List<XmlDocNode> Nodes { get; }
        public XmlNameTable NameTable
        {
            get
            {
                if (Nodes.Count == 0)
                    return null;
                else
                    return Nodes.First().OriginalNode.OwnerDocument.NameTable;
            }
        }

        public CVSection()
        {
            Nodes = new List<XmlDocNode>();
        }

        public void AddNode(XmlNode node)
        {
            if (node.Name == "w:p")
                Nodes.Add(new XmlDocParagraph(node));
            else if(node.Name == "w:tbl")
                Nodes.Add(new XmlDocTable(node));
        }
    }
}
