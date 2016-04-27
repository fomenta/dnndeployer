using Build.DotNetNuke.Deployer.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Authenticators;

namespace Build.Extensions.Tests.DotNetNuke
{
    [TestClass]
    public class ModuleAdminClientTests : BaseUnitTest<ModuleAdminClient>
    {
        #region Temporary Testing
        [TestMethod]
        public void SecuredMethodCall_Fail()
        {
            var client = new RestClient("http://dnndev.me/DesktopModules/DataExchange/API/Example");
            client.Authenticator = null;

            var request = new RestRequest("HelloWorld", Method.GET);
            var response = client.Execute(request);
            TestContext.WriteLine("StatusCode: [{0}] '{1}'", (int)response.StatusCode, response.StatusCode);
            TestContext.WriteLine("Response: '{0}'", response.Content);
        }
        [TestMethod]
        public void SecuredMethodCall_Success()
        {
            var client = new RestClient("http://dnndev.me/DesktopModules/DataExchange/API/Example");
            client.Authenticator = new HttpBasicAuthenticator("host", "abc123$");

            var request = new RestRequest("HelloWorld", Method.GET);
            var response = client.Execute(request);
            TestContext.WriteLine("StatusCode: [{0}] '{1}'", (int)response.StatusCode, response.StatusCode);
            TestContext.WriteLine("Response: '{0}'", response.Content);
        }

        public void Module_Sample_Calls_And_Different_Parameter_Types()
        {
            var client = new RestClient(DNN_URL);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("Dummy?moduleId={moduleId}", Method.GET);
            //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("moduleId", 123.ToString()); // replaces matching token in request.Resource

            // easily add HTTP Headers
            //request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            //request.AddFile(path);

            // execute the request
            var response = client.Execute(request);

            // display some data
            DisplayResponse(response);
        }
        #endregion


        [TestMethod]
        public void Module_1_1_IsDeployerInstalled()
        {
            var success = client.IsDeployerInstalled();
            TestContext.WriteLine("DNN_URL: {0}", DNN_URL);
            TestContext.WriteLine("IsDeployerInstalled: {0}", success);

            // display some data
            CheckAndDisplayResponse(client);
            Assert.IsTrue(success, "Success");
        }

        [TestMethod]
        public void Module_1_2_GetVersion()
        {
            var version = client.GetVersion();
            TestContext.WriteLine("GetVersion: {0}", version);

            // display some data
            CheckAndDisplayResponse(client);
            TestContext.WriteLine("{0}", version);
        }

        [TestMethod]
        public void Module_2_1_1_Blog_install() { ModuleAdminInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog); }

        [TestMethod]
        public void Module_2_1_2_Blog_uninstall() { ModuleAdminUninstall(SampleData.ModuleNameBlog); }


        [TestMethod]
        public void Module_2_1_3_Blog_Downgrade()
        {
            ModuleUninstall(SampleData.ModuleNameBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ false, SampleData.ModulePathBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ false, SampleData.ModulePathBlogDowngrade);
        }

        [TestMethod]
        public void Module_2_1_4_Blog_Upgrade()
        {
            TestContext.WriteLine("[ModuleUninstall]");
            ModuleUninstall(SampleData.ModuleNameBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlogUpgrade);
        }

        [TestMethod]
        public void Module_2_2_1_Articles_install() { ModuleAdminInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathSimpleArticle); }

        [TestMethod]
        public void Module_2_2_2_Articles_uninstall() { ModuleAdminUninstall(SampleData.ModuleNameSimpleArticle); }

        [TestMethod]
        public void Module_2_3_1_Both_install() { ModuleAdminInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog, SampleData.ModulePathSimpleArticle); }

        [TestMethod]
        public void Module_2_3_2_Both_uninstall() { ModuleAdminUninstall(SampleData.ModuleNameBlog, SampleData.ModuleNameSimpleArticle); }

        [TestMethod]
        public void Module_2_3_3_Custom_uninstall()
        {
            ModuleAdminUninstall("Aprobaciones", "AprobacionesClientes", "AprobacionesComplementosClientes",
                "ConsultaCartera", "ConsultaRadicado", "Module1", "DO", "Facture.Workflow.Designer",
                "InscripcionCliente", "ListadoAprobacionOperacion", "RequisitosAutorizaciones",
                "Traslados", "VentanasDinamicas", "ViewerReportServer"
                );
        }

        [TestMethod]
        public void Module_2_4_1_UserExport_install() { ModuleAdminInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathUserExport); }

        [TestMethod]
        public void Module_2_4_2_UserExport_uninstall() { ModuleAdminUninstall(SampleData.ModuleNameUserExport); }



        [TestMethod]
        public void Module_3_1_0_GetDesktop() { ModuleSecuredListDesktop("html"); }

        [TestMethod]
        public void Module_3_1_1_Blog_get() { ModuleSecuredList("Blog"); }

        [TestMethod]
        public void Module_3_1_2_Both_get() { ModuleSecuredList("Blog|Article"); }

        [TestMethod]
        public void Module_3_1_3_BuiltIn_get() { ModuleSecuredList("Con", builtIn: true); }

    }
}
