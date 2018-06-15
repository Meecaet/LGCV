using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches_Test
{
    [TestClass]
    public class CvGraphRepositoryTest
    {
        [TestMethod]
        public void ReadAllCvTest()
        {
            CVGraphRepository repo = new CVGraphRepository("Graphe_Essay", "graph_cv");
            List<CV> CVs = repo.GetAll();

            Assert.IsTrue(CVs.Count > 0);
        } 

    }
}
