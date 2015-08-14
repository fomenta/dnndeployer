using Build.Extensions.Helpers;
using Microsoft.TeamFoundation.Build.Workflow.Activities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Build.Extensions.Tests.Helpers
{
    [TestClass]
    public class FilteringTests : BaseUnitTest
    {
        private StringList ProjectsToBuild = new StringList(SampleData.AllProjectsToBuild);

        #region Tests
        [TestMethod]
        public void ApplyFilter_1_Empty() { VerifyApply(csvFilterList: "", expectedFilterCount: 94); }

        [TestMethod]
        public void ApplyFilter_2_PuertoBahia_Base() { VerifyApply(csvFilterList: "PuertoBahia.Base".ToLower(), expectedFilterCount: 1); }

        [TestMethod]
        public void ApplyFilter_3_Anulacion_Y_Complementos() { VerifyApply(csvFilterList: "AnulacionDocumentos,Complementos".ToLower(), expectedFilterCount: 2); }

        [TestMethod]
        public void ApplyFilter_4_1_AdminPermisos() { VerifyApply(csvFilterList: "AdminPermisos", expectedFilterCount: 0); }

        [TestMethod]
        public void ApplyFilter_4_2_AdminPermisosDongle() { VerifyApply(csvFilterList: "AdminPermisosDongles", expectedFilterCount: 1); }

        [TestMethod]
        public void ApplyFilter_5_Recalada() { VerifyApply(csvFilterList: "recaladas", expectedFilterCount: 1); }

        [TestMethod]
        public void ApplyFilter_6_Nominacion()
        {
            // Hay 2 pero deberia tomar el primero solamente: Nominaciones.sln, NominacionesGraphs.sln
            VerifyApply(csvFilterList: "nominaciones", expectedFilterCount: 1);
        }
        #endregion

        #region Helpers
        private void VerifyApply(string csvFilterList, int expectedFilterCount)
        {
            var output = Filtering.Apply(ProjectsToBuild, csvFilterList);

            Assert.IsNotNull(output, "Filtering.Apply() call");
            TestContext.WriteLine("Filter Count: {0}", output.Count);
            var index = 0;
            foreach (var item in output) { TestContext.WriteLine("[{0}] {1}", ++index, item); }
            Assert.AreEqual(expectedFilterCount, output.Count);
        }
        #endregion
    }
}
