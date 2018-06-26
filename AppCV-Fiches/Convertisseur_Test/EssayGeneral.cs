using System;
using System.Linq;
using System.IO;
using System.Text;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlHandler.Generation;

namespace Convertisseur_Test
{
    [TestClass]
    public class EssayGeneral
    {
        //Cet essai fera la generation d'un fichier xml selon les données d'un CV docx.
        //Après, il fera une comparaison avec un fichier modèdele dont le contenu a été revisé. De chaque fichier on extrait un hash. L'essai reussi si les hashs sont le même

        [TestMethod]
        public void TestDeGeneration()
        {
            DirectoryInfo extractedDirectory;
            Stream stream;
            string gabaritDirectoryPath = @"..\\..\\..\\Gabarits";

            try
            {                
                CVGenerator generator = new CVGenerator();                

                extractedDirectory = new DirectoryInfo($"{gabaritDirectoryPath}\\Modele_a_generer");
                if (extractedDirectory.Exists)
                    extractedDirectory.Delete(true);

                extractedDirectory = null;

                generator.ProcessCV(gabaritDirectoryPath);

                var md5 = System.Security.Cryptography.MD5.Create();
                byte[] byteMd5Modele, byteMd5Essay;

                string hashModele = string.Empty, hashEssay = string.Empty;                

                using (stream = new FileStream($"{gabaritDirectoryPath}\\Modele_de_CV.xml", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byteMd5Modele = md5.ComputeHash((Stream)stream);
                }

                using (stream = new FileStream($"{gabaritDirectoryPath}\\Modele_a_generer.xml", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byteMd5Essay = md5.ComputeHash((Stream)stream);
                }

                hashModele = Encoding.ASCII.GetString(byteMd5Modele);
                hashEssay = Encoding.ASCII.GetString(byteMd5Essay);

                Assert.AreEqual(hashModele, hashEssay);
            }
            finally
            {
                FileInfo fileInfo = new FileInfo($"{gabaritDirectoryPath}\\Modele_a_generer.xml");
                if (fileInfo.Exists)
                    fileInfo.Delete();
            }
        }



        [TestMethod]
        public void TestDAssemblageConseiller()
        {
            ConseillerGraphRepository conseillerGraphRepository;
            Conseiller conseiller;

            DirectoryInfo extractedDirectory;
            string gabaritDirectoryPath = @"..\\..\\..\\Gabarits";

            try
            {
                CVGenerator generator = new CVGenerator();
                conseillerGraphRepository = new ConseillerGraphRepository("Graphe_Essay", "graph_cv");

                extractedDirectory = new DirectoryInfo($"{gabaritDirectoryPath}\\Modele_a_generer");
                if (extractedDirectory.Exists)
                    extractedDirectory.Delete(true);

                extractedDirectory = null;

                conseillerGraphRepository.DeleteAllDocs();
                generator.ProcessCV(gabaritDirectoryPath);

                conseiller = conseillerGraphRepository.GetAll().First();

                Assert.IsNotNull(conseiller.Utilisateur);

                Assert.AreEqual(2, conseiller.FormationsScolaires.Count);

                Assert.AreEqual(1, conseiller.Associations.Count);

                Assert.AreEqual(9, conseiller.Formations.Count);

                Assert.IsNotNull(conseiller.Fonction);

                Assert.AreEqual(7, conseiller.DomaineDInterventions.Count);

                Assert.AreEqual(1, conseiller.Technologies.Count);

                Assert.AreEqual(2, conseiller.Employeurs.Count);
            }
            finally
            {
                FileInfo fileInfo = new FileInfo($"{gabaritDirectoryPath}\\Modele_a_generer.xml");
                if (fileInfo.Exists)
                    fileInfo.Delete();
            }
        }

        [TestMethod]
        public void TestDAssemblageMandat()
        {
            ConseillerGraphRepository conseillerGraphRepository;
            Conseiller conseiller;

            DirectoryInfo extractedDirectory;
            string gabaritDirectoryPath = @"..\\..\\..\\Gabarits";

            try
            {
                CVGenerator generator = new CVGenerator();
                conseillerGraphRepository = new ConseillerGraphRepository("Graphe_Essay", "graph_cv");

                extractedDirectory = new DirectoryInfo($"{gabaritDirectoryPath}\\Modele_a_generer");
                if (extractedDirectory.Exists)
                    extractedDirectory.Delete(true);

                extractedDirectory = null;

                conseillerGraphRepository.DeleteAllDocs();
                generator.ProcessCV(gabaritDirectoryPath);

                conseiller = conseillerGraphRepository.GetAll().First();

                foreach (Mandat mandat in conseiller.Mandats)
                {
                    Assert.IsNotNull(mandat.Projet);
                    Assert.AreEqual(7, mandat.Projet.Technologies.Count);
                    Assert.IsNotNull(mandat.Projet.Client);
                    Assert.IsNotNull(mandat.Projet.SocieteDeConseil);
                }
            }
            finally
            {
                FileInfo fileInfo = new FileInfo($"{gabaritDirectoryPath}\\Modele_a_generer.xml");
                if (fileInfo.Exists)
                    fileInfo.Delete();
            }
        }
    }
}
