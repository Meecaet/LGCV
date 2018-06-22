using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using DAL_CV_Fiches.Models;
using DAL_CV_Fiches.Repositories.Graph.Attributes;

namespace DAL_CV_Fiches_Test
{
    [TestClass]
    public class ClientGraphRepositoryTest
    {
        [TestMethod]
        public void TestAddVertex()
        {
            //try
            //{
            //    ClientGraphRepository repo = new ClientGraphRepository("Graphe_Essay", "graph_cv");
            //    List<Client> listOfClient;

            //    Guid identifier = Guid.NewGuid();

            //    Client client = new Client();
            //    client.Nom = identifier.ToString();

            //    repo.Add(client);

            //    listOfClient = repo.GetAll();

            //    Assert.IsTrue(listOfClient.Any(x => x.Nom == identifier.ToString()));
            //}
            //catch (System.Exception e)
            //{

            //    throw;
            //}
        }

        [TestMethod]
        public void TestAddVertexWithEdge()
        {
            try
            {
                //ClientGraphRepository repo = new ClientGraphRepository("Graphe_Essay", "graph_cv");
                //List<Client> listOfClient;

                //Guid identifier = Guid.NewGuid();

                //Client client = new Client();
                //client.Nom = identifier.ToString();

                //Mandat mandat = new Mandat();
                //mandat.Numero = "999";
                //mandat.Projet = "Essay de persistence";
                //mandat.Fonction = "Developeur";
                //mandat.Envenrgure = "159";
                //mandat.Description = "Loren Ipsum";

                //client.Mandats.Add(mandat);

                //repo.Add(client);

                //listOfClient = repo.GetAll();

                //Assert.IsTrue(listOfClient.Any(x => x.Nom == identifier.ToString()));
            }
            catch (System.Exception e)
            {

                throw;
            }


            ConseillerGraphRepository conseillerGraphRepository = new ConseillerGraphRepository("Graphe_Essay", "graph_cv");
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository("Graphe_Essay", "graph_cv");

            List<Conseiller> conseiller = conseillerGraphRepository.GetAll();
            List <Technologie> technologie = technologieGraphRepository.GetAll();
            List<bool> hasEdgesResults = new List<bool>();
            Edge edge = new Edge("Knows");

            foreach (var item in conseiller)
            {
                foreach (var item2 in technologie)
                {
                    hasEdgesResults.Add(conseillerGraphRepository.HasEdge(edge, item, item2));
                }
            }


            Assert.IsTrue(hasEdgesResults.Any(x => x));
            
        }

        [TestMethod]
        public void TestGetAllClients()
        {

        }

        [TestMethod]
        public void TestCreateEgdeWithProperty()
        {
            //try
            //{               

            //    Mandat mandat = new Mandat();
            //    mandat.Projet = "Create edge test";
            //    mandat.Numero = "1";

            //    Technologie tech1 = new Technologie();
            //    tech1.Nom = "C#";
            //    tech1.Description = "MS CSHARP";
            //    tech1.MoisDExperience = 30;

            //    Technologie tech2 = new Technologie();
            //    tech2.Nom = "Java";
            //    tech2.Description = "Oracle";

            //    mandat.Technologies.Add(tech1);
            //    mandat.Technologies.Add(tech2);

            //    MandatGraphRepository graphRepo = new MandatGraphRepository("Graphe_Essay", "graph_cv");
            //    int edgesBefore = graphRepo.CountEdges(), edgesAfter = 0;

            //    graphRepo.Add(mandat);

            //    edgesAfter = graphRepo.CountEdges();

            //    Assert.IsTrue(edgesAfter > edgesBefore);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }
    }
}
