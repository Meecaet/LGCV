using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
{
    public class ChildNodeToken : IXmlToken
    {
        private string nodeName;
        private XmlNamespaceManager namespaceManager;

        public void SetNodeName(string nodeName)
        {
            this.nodeName = nodeName;
        }

        public ChildNodeToken(XmlNameTable nametable)
        {
            namespaceManager = new XmlNamespaceManager(nametable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        }

        public bool Match(XmlNode node, out string identificant)
        {
            identificant = string.Empty;

            if (string.IsNullOrEmpty(nodeName))
                return false;

            if (node.SelectNodes($".//{nodeName}", namespaceManager).Count > 0)
            {
                identificant = nodeName;
                return true;
            }

            return false;
        }
    }
}
