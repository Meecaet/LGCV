using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlIdentification
{
    class CompostTextToken : IXmlToken
    {
        private Tuple<string, string> styles;
        private XmlNamespaceManager namespaceManager;

        public bool Match(XmlNode node, out string identificant)
        {
            throw new NotImplementedException();
        }
    }
}
