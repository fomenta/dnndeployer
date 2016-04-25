using RestSharp;
using System;
using System.IO;
using System.Net;

namespace Build.DotNetNuke.Deployer.Client
{
    [Obsolete("No se puede acceder de manera insegura al servidor.")]
    internal class DeployerUnsecuredClient : AdminBaseClient
    {
        #region Constants
        public const string REST_MODULE_GET = "moduleunsecured/get";
        public const string REST_MODULE_INSTALL = "moduleunsecured/install?deleteModuleFirstIfFound={deleteModuleFirstIfFound}";
        public const string REST_MODULE_UNINSTALL = "moduleunsecured/uninstall";
        #endregion

        #region Constructor
        public DeployerUnsecuredClient(string targetDnnRootUrl)
            : base()
        { TargetDotNetNukeRootUrl = targetDnnRootUrl; }
        #endregion

        #region REST for Module
        public bool ModuleInstall(bool deleteModuleFirstIfFound, params string[] modulesFilePath)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest(REST_MODULE_INSTALL, Method.PUT);
            request.AddUrlSegment("deleteModuleFirstIfFound", deleteModuleFirstIfFound.ToString());

            // add files to upload
            foreach (var item in modulesFilePath) { request.AddFile(Path.GetFileName(item), item); }

            // execute the request
            var response = client.Execute(request);

            LastRequestUrl = string.Format("{0}/{1}", ServiceUrl, request.Resource);
            LastResponse = response;
            return response.StatusCode == HttpStatusCode.OK;
        }

        public bool ModuleUninstall(params string[] moduleNames)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest(REST_MODULE_UNINSTALL + "?csvPackageNames={csvPackageNames}", Method.DELETE);

            // add modules to uninstall
            var modules = string.Join(",", moduleNames);
            request.AddUrlSegment("csvPackageNames", modules);

            // execute the request
            var response = client.Execute(request);

            LastRequestUrl = string.Format("{0}/{1}", ServiceUrl, request.Resource);
            LastResponse = response;
            return response.StatusCode == HttpStatusCode.OK;
        }

        public string ModuleGet(string filterPattern, bool builtIn = false)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest(REST_MODULE_GET + "?builtIn={builtIn}&filterPattern={filterPattern}", Method.GET);
            request.AddUrlSegment("builtIn", true.ToString()); // replaces matching token in request.Resource
            request.AddUrlSegment("filterPattern", filterPattern); // replaces matching token in request.Resource
            var response = client.Execute(request);

            LastRequestUrl = string.Format("{0}/{1}", ServiceUrl, request.Resource);
            LastResponse = response;
            return response.Content;
        }

        #endregion
    }
}
