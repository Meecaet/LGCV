using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CVModel.XmlIdentification
{
    public interface IXmlToken
    {
        bool Match(XmlNode node, out string identificant);
    }
}
