using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlEntities
{
    public abstract class XmlDocNode
    {
        protected XmlNode xmlNode;
        protected XmlNamespaceManager namespaceManager;

        public XmlNode OriginalNode { get { return xmlNode; } }

        public XmlDocNode(XmlNode xmlNode)
        {
            this.xmlNode = xmlNode;

            namespaceManager = new XmlNamespaceManager(xmlNode.OwnerDocument.NameTable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        }
    }
}
