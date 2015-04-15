using Build.Extensions.DotNetNuke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System.Net;

namespace Build.Extensions.Tests.DotNetNuke
{
    [TestClass]
    public class ModuleUnsecuredClientTests : BaseUnitTest
    {
        [TestMethod]
        public void REST_1_1_IsDeployerInstalled()
        {
            var client = new DeployerUnsecuredClient(DNN_URL);
            var success = client.IsDeployerInstalled();
            TestContext.WriteLine("IsDeployerInstalled: {0}", success);

            // display some data
            CheckAndDisplayResponse(client);
            Assert.IsTrue(success, "Success");
        }

        [TestMethod]
        public void REST_1_2_GetVersion()
        {
            var client = new DeployerUnsecuredClient(DNN_URL);
            var version = client.GetVersion();
            TestContext.WriteLine("{0}", version);

            // display some data
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void REST_1_3_Dummy()
        {
            var client = new RestClient(DNN_URL + "/DesktopModules/Deployer/API");
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

            // raw content as string
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void REST_2_1_1_module_unsecured_Blog_install() { ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog); }

        [TestMethod]
        public void REST_2_1_2_module_unsecured_Blog_uninstall() { ModuleUninstall(SampleData.ModuleNameBlog); }

        [TestMethod]
        public void REST_2_1_3_module_unsecured_Blog_Downgrade()
        {
            ModuleUninstall(SampleData.ModuleNameBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ false, SampleData.ModulePathBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ false, SampleData.ModulePathBlogDowngrade);
        }

        [TestMethod]
        public void REST_2_1_4_module_unsecured_Blog_Upgrade()
        {
            TestContext.WriteLine("[ModuleUninstall]");
            ModuleUninstall(SampleData.ModuleNameBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog);
            ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlogUpgrade);
        }

        [TestMethod]
        public void REST_2_2_1_module_unsecured_Articles_install() { ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathSimpleArticle); }

        [TestMethod]
        public void REST_2_2_2_module_unsecured_Articles_uninstall() { ModuleUninstall(SampleData.ModuleNameSimpleArticle); }

        [TestMethod]
        public void REST_2_3_1_module_unsecured_Both_install() { ModuleInstall(/*deleteModuleFirstIfFound:*/ true, SampleData.ModulePathBlog, SampleData.ModulePathSimpleArticle); }

        [TestMethod]
        public void REST_2_3_2_module_unsecured_Both_uninstall() { ModuleUninstall(SampleData.ModuleNameBlog, SampleData.ModuleNameSimpleArticle); }

        [TestMethod]
        public void REST_2_3_3_module_unsecured_Custom_uninstall()
        {
            ModuleUninstall("Aprobaciones", "AprobacionesClientes", "AprobacionesComplementosClientes",
                "ConsultaCartera", "ConsultaRadicado", "Module1", "DO", "Facture.Workflow.Designer",
                "InscripcionCliente", "ListadoAprobacionOperacion", "RequisitosAutorizaciones",
                "Traslados", "VentanasDinamicas", "ViewerReportServer"
                );
        }

        [TestMethod]
        public void REST_3_1_1_module_unsecured_Blog_get() { ModuleGet("Blog"); }

        [TestMethod]
        public void REST_3_1_2_module_unsecured_Both_get() { ModuleGet("Blog|Article"); }

        [TestMethod]
        public void REST_3_1_3_module_unsecured_BuiltIn_get() { ModuleGet("Con", builtIn: true); }


    }
}
