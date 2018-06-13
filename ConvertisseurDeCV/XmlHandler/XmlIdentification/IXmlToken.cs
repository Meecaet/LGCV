using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlHandler.XmlIdentification
{
    public interface IXmlToken
    {
        /// <summary>
        /// Méthode qui dit si un nœud répresent le début d'un section
        /// </summary>
        /// <param name="node">Nœud en train d'être lu</param>
        /// <param name="identificant">Non de la section qui, dans le cas où le "match" est vrai, sera retourné</param>
        /// <returns>Indicatif si le nœud passe ou non par la véerification</returns>
        bool Match(XmlNode node, out string identificant);
    }
}
