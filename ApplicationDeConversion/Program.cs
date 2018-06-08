using CVModel.Domain;
using CVModel.XmlEntities;
using CVModel.XmlIdentification;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ApplicationDeConversion
{
    class Program
    {      
        static void Main(string[] args)
        {
            string path = args[0];
            ProcessCV(path);
        }

        private static void GenerateCVXml(string documentXmlPath, string outputPath)
        {
            List<CVSection> Sections;

            #region Get XML Nodes

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(documentXmlPath);

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

            CV nouveauCV = new CV();
            nouveauCV.AssemblerCV(Sections);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CV));
            using (Stream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlSerializer.Serialize(fileStream, nouveauCV);
            }
        }

        private static void ProcessCV(string path)
        {
            DirectoryInfo directoryInfo, extractedDirectoryInfo;
            string currentExtractFolder;

            FileInfo[] filesInDirectory;
            Dictionary<string, string> generatedFolders;

            FileInfo xmlDocumetFile;

            directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists)
            {
                filesInDirectory = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);

                if (filesInDirectory.Length > 0)
                {
                    generatedFolders = new Dictionary<string, string>();

                    foreach (var item in filesInDirectory)
                    {
                        currentExtractFolder = string.Format(@"{0}\{1}", directoryInfo.FullName, item.Name.Replace(".docx", ""));
                        generatedFolders.Add(item.FullName, currentExtractFolder);

                        Console.WriteLine($"{item.FullName} => {currentExtractFolder}");
                        ZipFile.ExtractToDirectory(item.FullName, currentExtractFolder);

                        extractedDirectoryInfo = new DirectoryInfo(string.Format(@"{0}\word", currentExtractFolder));
                        xmlDocumetFile = extractedDirectoryInfo.GetFiles("document.xml", SearchOption.TopDirectoryOnly).DefaultIfEmpty(null).FirstOrDefault();
                        GenerateCVXml(xmlDocumetFile.FullName, item.FullName.Replace(".docx", ".xml"));

                        xmlDocumetFile = null;
                        extractedDirectoryInfo.Parent.Delete(true);
                    }
                }
            }
        }

        //private static void ProcessCVFile()
        //{
        //    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    doc.Load(@"C:\Docs to zip\Caetano Mateus - CV (2018-05-28)\word\document.xml");

        //    XmlNode rootNnode = doc.LastChild.LastChild;
        //    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
        //    namespaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
        //    namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

        //    List<XmlNode> nodes = new List<XmlNode>();
        //    foreach (XmlNode item in rootNnode.ChildNodes)
        //    {
        //        if (item.Name.Contains("w:p") && string.IsNullOrEmpty(item.InnerText))
        //            continue;

        //        nodes.Add(item);
        //    }


        //    CV newCV = new CV();
        //    stringBuilder = new StringBuilder();

        //    //Get Nom et titre

        //    var nodeNom = nodes.First(x => x.Name == "w:tbl");
        //    var cvData = nodeNom.SelectNodes(".//w:t", namespaceManager);

        //    foreach (XmlNode item in cvData)
        //    {
        //        if (string.IsNullOrEmpty(newCV.Nom))
        //            newCV.Nom = item.InnerText;
        //        else
        //            newCV.Titre = item.InnerText;
        //    }

        //    //end

        //    nodes.Remove(nodeNom);

        //    //Get description

        //    var nodeDescription = nodes.TakeWhile(x => x.Name == "w:p").ToList();
        //    foreach (XmlNode item in nodeDescription)
        //    {
        //        var descData = item.SelectNodes(".//w:t", namespaceManager);
        //        for (int i = 0; i < descData.Count; i++)
        //        {
        //            if (descData.Item(i).ParentNode.SelectNodes(".//w:sz[@w:val = \"22\"]", namespaceManager).Count == 0)
        //                stringBuilder.Append(descData.Item(i).InnerText);
        //        }

        //    }

        //    newCV.Description = stringBuilder.ToString();

        //    //end

        //    nodes.RemoveAll(x => nodeDescription.Contains(x));

        //    //Get domaines d`interventions

        //    newCV.DomaineDIntervention = new List<string>();
        //    var domaines = nodes.First(x => x.Name == "w:tbl");
        //    var domaineData = domaines.SelectNodes(".//w:p", namespaceManager);
        //    foreach (XmlNode item in domaineData)
        //    {
        //        newCV.DomaineDIntervention.Add(item.InnerText);
        //    }

        //    //end

        //    nodes.Remove(domaines);

        //    //Add Formation Academique

        //    newCV.FormationAcademique = new FormationAcademique();
        //    newCV.FormationAcademique.Itens = new List<FormationAcademique>();
        //    var formations = nodes.First(x => x.Name == "w:tbl");
        //    var academiqueData = formations.SelectNodes(".//w:tr/w:tc[1]", namespaceManager);
        //    foreach (XmlNode item in academiqueData)
        //    {
        //        FormationAcademique formItem = new FormationAcademique();
        //        var descData = item.SelectNodes(".//w:p", namespaceManager);
        //        for (int i = 0; i < descData.Count; i++)
        //        {
        //            if (!descData.Item(i).InnerText.Contains("FORMATION ACADÉMIQUE") && !string.IsNullOrEmpty(descData.Item(i).InnerText))
        //                if (string.IsNullOrEmpty(formItem.Titre))
        //                {
        //                    formItem.Titre = descData.Item(i).InnerText;
        //                }
        //                else
        //                {
        //                    formItem.Instituition = descData.Item(i).InnerText;
        //                }

        //        }

        //        if (!string.IsNullOrEmpty(formItem.Titre))
        //            newCV.FormationAcademique.Itens.Add(formItem);
        //    }

        //    //Add certifications

        //    newCV.Certifications = new List<string>();
        //    var certificationsData = formations.SelectNodes(".//w:tr/w:tc[2]", namespaceManager);
        //    foreach (XmlNode item in certificationsData)
        //    {
        //        var descData = item.SelectNodes(".//w:p", namespaceManager);
        //        for (int i = 0; i < descData.Count; i++)
        //        {
        //            if (!descData.Item(i).InnerText.Contains("CERTIFICATIONS") && !string.IsNullOrEmpty(descData.Item(i).InnerText))
        //                newCV.Certifications.Add(descData.Item(i).InnerText);
        //        }
        //    }

        //    //end

        //    nodes.Remove(formations);

        //    //TODO - add Resume d`interventions

        //    aNode = nodes.First();
        //    nodes.Remove(aNode);

        //    aNode = nodes.First(x => x.Name == "w:tbl");
        //    nodes.Remove(aNode);

        //    //end TODO

        //    //Add Employeur

        //    bool hasTitre1 = true, hasClient = true;
        //    newCV.Employeurs = new List<Employeur>();

        //    do
        //    {
        //        aNode = nodes.First();
        //        foreach (XmlAttribute item in aNode.FirstChild.FirstChild.Attributes)
        //        {
        //            if (item.Value != "Titre1")
        //            {
        //                hasTitre1 = false;
        //                break;
        //            }
        //        }

        //        Employeur emp = new Employeur();
        //        emp.Nom = aNode.InnerText;

        //        nodes.Remove(aNode);
        //        Client cli = null;

        //        do
        //        {
        //            aNode = nodes.First();

        //            if (aNode.Name == "w:p")
        //            {
        //                cli = new Client();
        //                cli.Nom = aNode.InnerText;

        //                nodes.Remove(aNode);
        //            }
        //            else
        //            {

        //            }


        //            if (aNode.Name != "w:tbl")
        //                hasClient = false;

        //            cli.Mandats = new List<Mandat>();
        //            Mandat aMandat = new Mandat();

        //            var clientData = aNode.SelectNodes(".//w:tr/w:tc[2]/w:p");
        //            foreach (XmlNode item in clientData)
        //            {
        //                if (string.IsNullOrEmpty(aMandat.Projet))
        //                    aMandat.Numero = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Projet))
        //                    aMandat.Projet = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Envenrgure))
        //                    aMandat.Envenrgure = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Fonction))
        //                    aMandat.Fonction = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Periode))
        //                    aMandat.Periode = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Efforts))
        //                    aMandat.Efforts = item.InnerText;
        //                else if (string.IsNullOrEmpty(aMandat.Reference))
        //                    aMandat.Reference = item.InnerText;
        //            }

        //            //TODO - Séparer les données de la descrition

        //            nodes.Remove(aNode);
        //            stringBuilder.Clear();

        //            var descriptionData = nodes.TakeWhile(x => x.Name == "w:p");
        //            foreach (XmlNode item in descriptionData)
        //                stringBuilder.AppendLine(item.InnerText);

        //            aMandat.Description = stringBuilder.ToString();
        //            stringBuilder.Clear();

        //            cli.Mandats.Add(aMandat);
        //            emp.Clients.Add(cli);

        //        } while (hasTitre1 && hasClient);



        //        while (true)
        //        {

        //        }





        //    } while (hasTitre1);

        //    {

        //    }

        //    //end

        //    Console.ReadKey();
        //}

    }
}
