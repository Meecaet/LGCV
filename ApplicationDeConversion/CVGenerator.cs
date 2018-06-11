using CVModel.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XmlHandler.Services;
using XmlHandler.XmlEntities;
using XmlHandler.XmlIdentification;

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

        /// <summary>
        /// Fait l'extraction et la conversion d'un CV LGS vers un fichier en format xml depuis un dossier avec des fichiers docx
        /// </summary>
        /// <param name="path">Chemin du dossier où sont les fichiers de CV en format docx</param>
        public void ProcessCV(string path)
        {
            string extractedXmlText, currentExtractFolder;

            DirectoryInfo directoryInfo;
            FileInfo[] filesInDirectory;
            DocxExtractor docxExtractor = new DocxExtractor();

            directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists)
            {
                //Prendre seulement les fichiers docx
                filesInDirectory = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);

                if (filesInDirectory.Length > 0)
                {
                    foreach (FileInfo file in filesInDirectory)
                    {                        
                        //Nous avons besoin d'un ficher dont le contenu est le text du docx (.\\word\\document.xml)
                        extractedXmlText = docxExtractor.ExtractDocxTextXml(file);

                        //Fait la génération d'un xml plus structuré
                        GenerateCVXml(extractedXmlText, file.FullName.Replace(file.Extension, ".xml"));

                        //Efface le dossier généré par l'extraction
                        currentExtractFolder = file.FullName.Replace(file.Extension, "");
                        DirectoryInfo extractionFolder = new DirectoryInfo(currentExtractFolder);
                        extractionFolder.Delete(true);                  
                    }
                }
            }
        }

    }
}
