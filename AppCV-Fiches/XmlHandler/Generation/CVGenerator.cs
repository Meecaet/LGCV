#define DEBUG

//using CVModel.Domain;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
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

namespace XmlHandler.Generation
{
    public class CVGenerator
    {
        private UtilisateurGraphRepository utilisateurRepo;


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

            utilisateurRepo = new UtilisateurGraphRepository("Graph_CV", "CVs");

            directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists)
            {
                //Prendre seulement les fichiers docx
                filesInDirectory = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);

                if (filesInDirectory.Length > 0)
                {
#if DEBUG
                    utilisateurRepo.DeleteAllDocs();
#endif

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

        private void GenerateCVXml(string documentXmlPath, string outputPath)
        {
            string currentIdent = string.Empty;

            SectionsExtractor CvSectionsExtractor = new SectionsExtractor();          
            List<XmlNode> nodes = CvSectionsExtractor.GetCVNodes(documentXmlPath);

            CVFactory cVFactory = new CVFactory();
            Utilisateur newUtilisateur = cVFactory.CreateConseiller(nodes);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Conseiller));
            using (Stream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlSerializer.Serialize(fileStream, newUtilisateur);
            }

            PersistCV(newUtilisateur);
        }

        private void PersistCV(Utilisateur utilisateur)
        {
            utilisateurRepo.Add(utilisateur);
        }
    }
}
