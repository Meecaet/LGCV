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
using System.Xml.Serialization;

namespace ApplicationDeConversion
{
    public class CVGenerator
    {
        private void GenerateCVXml(string documentXmlPath, string outputPath)
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

        public void ProcessCV(string path)
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

    }
}
