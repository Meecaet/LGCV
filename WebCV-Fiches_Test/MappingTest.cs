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
            Conseiller conseiller = new ConseillerGraphRepository("Graph_CV", "CVs").GetOne("bd7a4b01-51dc-45b7-8868-73901b366338");
            CVViewModel cVViewModel = mapper.Map(conseiller);

            Assert.IsNotNull(cVViewModel);
            Assert.AreEqual(conseiller.Fonction.Description, cVViewModel.Fonction);
        }
    }
}
