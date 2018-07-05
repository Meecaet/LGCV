using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlHandler.XmlEntities
{
    public class XmlDocRow : XmlDocNode
    {
        public XmlDocRow(XmlNode xmlNode) : base(xmlNode)
        {
            if (xmlNode.Name != "w:r")
                throw new ArgumentException("Invalid xml node");
        }

        public string GetText()
        {
            return xmlNode.InnerText;
        }

        public bool IsBold()
        {
            return xmlNode.SelectNodes(".//w:b", this.namespaceManager).Count > 0;                
        }

        public bool IsItalic()
        {
            return xmlNode.SelectNodes(".//w:i", this.namespaceManager).Count > 0;
        }
    }
}
