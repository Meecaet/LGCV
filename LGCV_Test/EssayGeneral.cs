using System;
using System.IO;
using System.Text;
using ApplicationDeConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LGCV_Test
{
    [TestClass]
    public class EssayGeneral
    {
        [TestMethod]
        public void TestDeGeneration()
        {
            try
            {
                DirectoryInfo extractedDirectory;
                Stream stream;
                CVGenerator generator = new CVGenerator();

                extractedDirectory = new DirectoryInfo(@"..\\..\\Gabarits\\Modele_a_generer");
                if (extractedDirectory.Exists)
                    extractedDirectory.Delete(true);

                extractedDirectory = null;

                generator.ProcessCV(@"..\\..\\Gabarits");

                var md5 = System.Security.Cryptography.MD5.Create();
                byte[] byteMd5Modele, byteMd5Essay;

                string hashModele = string.Empty, hashEssay = string.Empty;                

                using (stream = new FileStream(@"..\\..\\Gabarits\\Modele_de_CV.xml", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byteMd5Modele = md5.ComputeHash((Stream)stream);
                }

                using (stream = new FileStream(@"..\\..\\Gabarits\\Modele_a_generer.xml", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byteMd5Essay = md5.ComputeHash((Stream)stream);
                }

                hashModele = Encoding.ASCII.GetString(byteMd5Modele);
                hashEssay = Encoding.ASCII.GetString(byteMd5Essay);

                Assert.AreEqual(hashModele, hashEssay);
            }
            finally
            {
                FileInfo fileInfo = new FileInfo(@"..\\..\\Gabarits\\Modele_a_generer.xml");
                if (fileInfo.Exists)
                    fileInfo.Delete();
            }
        }
    }
}
