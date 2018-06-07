using CVModel.Domain;
using CVModel.XmlEntities;
using CVModel.XmlIdentification;
using System;
using System.Collections.Generic;
using System.IO;
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
        static XmlNode aNode;

        static List<CVSection> Sections;

        static void Main(string[] args)
        {            
            #region Get XML Nodes

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Docs to zip\Caetano Mateus - CV (2018-05-28)\word\document.xml");

            XmlNode rootNnode = doc.LastChild.LastChild;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode item in rootNnode.ChildNodes)
            {
                if (item.Name.Contains("w:p") && string.IsNullOrEmpty(item.InnerText))
                    continue;

                nodes.Add(item);
            }

            #endregion
            
            XmlNode first = null;
            string currentIdent = string.Empty;

            Sections = new List<CVSection>();
            List<IXmlToken> validationTokens = new List<IXmlToken>();

            validationTokens.Add(new TextToken());
            validationTokens.Add(new FormatationToken(doc.NameTable));
            CVSection currentCVSection = new CVSection();
            currentCVSection.Identifiant = "IDENTIFICATION";

            while (nodes.Count > 0)
            {
                first = nodes.First();
                nodes.Remove(first);

                if (validationTokens.Any(x => x.Match(first, out currentIdent)))
                {
                    Sections.Add(currentCVSection);

                    currentCVSection = new CVSection();
                    currentCVSection.Identifiant = currentIdent;
                    currentCVSection.AddNode(first);
                    
                    currentIdent = string.Empty;
                }
                else
                {
                    currentCVSection.AddNode(first);
                }
            }

            Sections.Add(currentCVSection);

            //using (StreamWriter sw = new StreamWriter(@"C:\\Docs to zip\\Sections.txt", false))
            //{
            //    foreach (var item in Sections)
            //    {
            //        sw.WriteLine("IDENTIFIANT -> " + item.Identifiant);
            //        foreach (var item2 in item.Nodes)
            //        {
            //            sw.WriteLine(item2.InnerText);
            //        }

            //        sw.WriteLine(string.Empty);
            //        sw.WriteLine("==================================================");
            //        sw.WriteLine(string.Empty);
            //    }
            //}

            CV nouveauCV = new CV();
            nouveauCV.AssemblerCV(Sections);

            Console.ReadKey();
        }

        private static void ProcessCVFile()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Docs to zip\Caetano Mateus - CV (2018-05-28)\word\document.xml");

            XmlNode rootNnode = doc.LastChild.LastChild;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode item in rootNnode.ChildNodes)
            {
                if (item.Name.Contains("w:p") && string.IsNullOrEmpty(item.InnerText))
                    continue;

                nodes.Add(item);
            }


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
                    if (descData.Item(i).ParentNode.SelectNodes(".//w:sz[@w:val = \"22\"]", namespaceManager).Count == 0)
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

            //Add Formation Academique

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

            //Add certifications

            newCV.Certifications = new List<string>();
            var certificationsData = formations.SelectNodes(".//w:tr/w:tc[2]", namespaceManager);
            foreach (XmlNode item in certificationsData)
            {
                var descData = item.SelectNodes(".//w:p", namespaceManager);
                for (int i = 0; i < descData.Count; i++)
                {
                    if (!descData.Item(i).InnerText.Contains("CERTIFICATIONS") && !string.IsNullOrEmpty(descData.Item(i).InnerText))
                        newCV.Certifications.Add(descData.Item(i).InnerText);
                }
            }

            //end

            nodes.Remove(formations);

            //TODO - add Resume d`interventions

            aNode = nodes.First();
            nodes.Remove(aNode);

            aNode = nodes.First(x => x.Name == "w:tbl");
            nodes.Remove(aNode);

            //end TODO

            //Add Employeur

            bool hasTitre1 = true, hasClient = true;
            newCV.Employeurs = new List<Employeur>();

            do
            {
                aNode = nodes.First();
                foreach (XmlAttribute item in aNode.FirstChild.FirstChild.Attributes)
                {
                    if (item.Value != "Titre1")
                    {
                        hasTitre1 = false;
                        break;
                    }
                }

                Employeur emp = new Employeur();
                emp.Nom = aNode.InnerText;

                nodes.Remove(aNode);
                Client cli = null;

                do
                {
                    aNode = nodes.First();

                    if (aNode.Name == "w:p")
                    {
                        cli = new Client();
                        cli.Nom = aNode.InnerText;

                        nodes.Remove(aNode);
                    }
                    else
                    {

                    }


                    if (aNode.Name != "w:tbl")
                        hasClient = false;

                    cli.Mandats = new List<Mandat>();
                    Mandat aMandat = new Mandat();

                    var clientData = aNode.SelectNodes(".//w:tr/w:tc[2]/w:p");
                    foreach (XmlNode item in clientData)
                    {
                        if (string.IsNullOrEmpty(aMandat.Projet))
                            aMandat.Numero = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Projet))
                            aMandat.Projet = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Envenrgure))
                            aMandat.Envenrgure = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Fonction))
                            aMandat.Fonction = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Periode))
                            aMandat.Periode = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Efforts))
                            aMandat.Efforts = item.InnerText;
                        else if (string.IsNullOrEmpty(aMandat.Reference))
                            aMandat.Reference = item.InnerText;
                    }

                    //TODO - Séparer les données de la descrition

                    nodes.Remove(aNode);
                    stringBuilder.Clear();

                    var descriptionData = nodes.TakeWhile(x => x.Name == "w:p");
                    foreach (XmlNode item in descriptionData)
                        stringBuilder.AppendLine(item.InnerText);

                    aMandat.Description = stringBuilder.ToString();
                    stringBuilder.Clear();

                    cli.Mandats.Add(aMandat);
                    emp.Clients.Add(cli);

                } while (hasTitre1 && hasClient);



                while (true)
                {

                }





            } while (hasTitre1);

            {

            }

            //end

            Console.ReadKey();
        }

    }
}
