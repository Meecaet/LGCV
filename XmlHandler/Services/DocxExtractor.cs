using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlHandler.Services
{
    public class DocxExtractor
    {

        public void ExtractDocx(string FilePath, string OutputPath)
        {
            FileInfo File;
            File = new FileInfo(FilePath);

            if (File.Exists && File.Extension == ".docx")
            {
                ZipFile.ExtractToDirectory(File.FullName, OutputPath);
            }
        }

        public void ExtractDocx(FileInfo File, string OutputPath)
        {
            if (File.Exists && File.Extension == ".docx")
            {
                ZipFile.ExtractToDirectory(File.FullName, OutputPath);
            }
        }

        public string ExtractDocxTextXml(FileInfo File)
        {
            string outputDirectory = File.FullName.Replace(File.Extension, "");
            ExtractDocx(File, outputDirectory);

            return $"{outputDirectory}\\word\\document.xml";
        }

        public Dictionary<string, string> ExtractDocxBatch(string DirectoryPath, string OutputPath)
        {
            string currentExtractFolder;

            DirectoryInfo directoryInfo;
            FileInfo[] filesInDirectory;
            Dictionary<string, string> generatedFolders = null;

            directoryInfo = new DirectoryInfo(DirectoryPath);

            if (directoryInfo.Exists)
            {
                filesInDirectory = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);

                if (filesInDirectory.Length > 0)
                {
                    generatedFolders = new Dictionary<string, string>();

                    foreach (FileInfo file in filesInDirectory)
                    {
                        currentExtractFolder = string.Format(@"{0}\{1}", directoryInfo.FullName, file.Name.Replace(".docx", ""));
                        generatedFolders.Add(file.FullName, currentExtractFolder);

                        ZipFile.ExtractToDirectory(file.FullName, currentExtractFolder);
                    }
                }
            }

            return generatedFolders;
        }

    }
}
