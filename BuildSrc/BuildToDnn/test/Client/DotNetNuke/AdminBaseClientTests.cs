using Build.DotNetNuke.Deployer.Client;
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
            var client = new AdminBaseClient(DNN_URL);
            var success = client.IsDeployerInstalled();
            TestContext.WriteLine("IsDeployerInstalled: {0}", success);

            // display some data
            CheckAndDisplayResponse(client);
            Assert.IsTrue(success, "Success");
        }

        [TestMethod]
        public void REST_1_2_GetVersion()
        {
            var client = new AdminBaseClient(DNN_URL);
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
    }
}
