using CVModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ApplicationDeConversion
{
    class Program
    {
        static XDocument xmlDocument, xmlFirstNode;
        static StringBuilder stringBuilder;

        static void Main(string[] args)
        {

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Docs to zip\Caetano Mateus - CV (2018-05-28)\word\document.xml");

            XmlNode rootNnode = doc.LastChild.LastChild;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode item in rootNnode.ChildNodes)
                nodes.Add(item);

            CV newCV = new CV();
            stringBuilder = new StringBuilder();

            //Get Nom et titre

            var nodeNom = nodes.First(x => x.Name == "w:tbl");
            var cvData = nodeNom.SelectNodes(".//w:t", namespaceManager);

            foreach (XmlNode item in cvData)
            {
                if (string.IsNullOrEmpty(newCV.Nom))
                    newCV.Nom = item.InnerText;
                else
                    newCV.Titre = item.InnerText;
            }

            //end

            nodes.Remove(nodeNom);

            //Get description

            var nodeDescription = nodes.TakeWhile(x => x.Name == "w:p").ToList();
            foreach (XmlNode item in nodeDescription)
            {
                var descData = item.SelectNodes(".//w:t", namespaceManager);
                for (int i = 0; i < descData.Count; i++)
                {
                    if(descData.Item(i).ParentNode.SelectNodes(".//w:sz[@w:val = \"22\"]", namespaceManager).Count == 0)
                        stringBuilder.Append(descData.Item(i).InnerText);
                }
                
            }

            newCV.Description = stringBuilder.ToString();

            //end

            nodes.RemoveAll(x => nodeDescription.Contains(x));

            //Get domaines d`interventions

            newCV.DomaineDIntervention = new List<string>();
            var domaines = nodes.First(x => x.Name == "w:tbl");
            var domaineData = domaines.SelectNodes(".//w:p", namespaceManager);
            foreach (XmlNode item in domaineData)
            {
                newCV.DomaineDIntervention.Add(item.InnerText);
            }

            //end

            nodes.Remove(domaines);

            newCV.FormationAcademique = new FormationAcademique();
            newCV.FormationAcademique.Itens = new List<FormationAcademiqueItem>();
            var formations = nodes.First(x => x.Name == "w:tbl");
            var academiqueData = formations.SelectNodes(".//w:tr/w:tc[1]", namespaceManager);
            foreach (XmlNode item in academiqueData)
            {
                FormationAcademiqueItem formItem = new FormationAcademiqueItem();
                var descData = item.SelectNodes(".//w:p", namespaceManager);
                for (int i = 0; i < descData.Count; i++)
                {
                    if (!descData.Item(i).InnerText.Contains("FORMATION ACADÉMIQUE") && !string.IsNullOrEmpty(descData.Item(i).InnerText))
                        if (string.IsNullOrEmpty(formItem.Titre))
                        {
                            formItem.Titre = descData.Item(i).InnerText;
                        }
                        else
                        {
                            formItem.Instituition = descData.Item(i).InnerText;
                        }

                }

                if (!string.IsNullOrEmpty(formItem.Titre))
                    newCV.FormationAcademique.Itens.Add(formItem);
            }

            Console.ReadKey();
        }
    }
}
