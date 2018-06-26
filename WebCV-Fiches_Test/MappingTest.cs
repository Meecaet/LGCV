using System.Linq;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCV_Fiches.Helpers;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches_Test
{
    [TestClass]
    public class MappingTest
    {
        [TestMethod]
        public void TestMappintDomainModelToViewModel()
        {
            CVMapper mapper = new CVMapper();
            Utilisateur utilisateur = new UtilisateurGraphRepository("Graph_CV", "CVs").GetOne("7b495fbb-6346-47e8-84dc-84fd3d2fc354");
            CVViewModel cVViewModel = mapper.Map(utilisateur);

            Assert.IsNotNull(cVViewModel);
            Assert.AreEqual(utilisateur.Conseiller.Fonction.Description, cVViewModel.Fonction);
        }

        [TestMethod]
        public void TestMappintViewModelToDomainModel()
        {
            CVMapper mapper = new CVMapper();
            Utilisateur utilisateur = new UtilisateurGraphRepository("Graph_CV", "CVs").GetOne("7b495fbb-6346-47e8-84dc-84fd3d2fc354");
            CVViewModel cVViewModel = mapper.Map(utilisateur);

            Utilisateur newUtilisateur = mapper.Map(cVViewModel);

            Assert.IsNotNull(newUtilisateur);
            Assert.AreEqual(newUtilisateur.Conseiller.Fonction.Description, cVViewModel.Fonction);
        }
    }
}
