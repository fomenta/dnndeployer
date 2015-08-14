using Build.DotNetNuke.Deployer.Library;
using Build.Extensions.DotNetNuke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Build.Extensions.Tests.DotNetNuke
{
    /// <summary>
    /// Parameter name: portalAlias
    /// </summary>
    [TestClass]
    public class PageModuleAdminClientTests : BaseUnitTest
    {
        [TestMethod]
        public void PageModule_1_1_IsDeployerInstalled()
        {
            var client = new PageAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var success = client.IsDeployerInstalled();

            // display some data
            CheckAndDisplayResponse(client);
            Assert.IsTrue(success, "Success");
        }

        [TestMethod]
        public void PageModule_1_2_GetVersion()
        {
            var client = new PageAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var version = client.GetVersion();
            TestContext.WriteLine("Version: {0}", version);

            // display some data
            CheckAndDisplayResponse(client);
        }

        #region Modules on Page
        [TestMethod]
        public void PageModule_2_1_2_Add()
        {
            var client = new PageModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var moduleID = client.Add(new PageModuleDto("forDNN.UsersExportImport"
                //                                      , paneName: "leftPane"
                                                        , moduleTitle: "User Import/Export"
                                                        , settingName: "DefaultUserControl"
                                                        , settingValue: "ControlKeyA"
                                                ),
                                                new PageGetRequest(tabFullName: "Testing UserControls")
                                                , clearExisting: true);
            CheckAndDisplayResponse(client);

            Assert.IsTrue(moduleID > 0);
        }

        [TestMethod]
        public void PageModule_2_1_2_Get()
        {
            var client = new PageModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var list = client.Get(new PageGetRequest(tabFullName: "Testing UserControls"));
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void PageModule_2_1_3_Clear()
        {
            var client = new PageModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var cleared = client.Clear(new PageGetRequest(tabFullName: "Testing UserControls"));
            CheckAndDisplayResponse(client);

            Assert.IsTrue(cleared);
        }
        #endregion
    }
}
