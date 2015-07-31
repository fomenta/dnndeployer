using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Build.Extensions.DotNetNuke
{
    public class ModuleAdminClient : AdminBaseClient
    {
        #region Constants
        public const string REST_MODULE_GET = "module/get?filterPattern={filterPattern}&builtIn={builtIn}";
        public const string REST_MODULE_GET_DESKTOP = "module/GetDesktop?filterPattern={filterPattern}";
        public const string REST_MODULE_INSTALL = "module/install?deleteModuleFirstIfFound={deleteModuleFirstIfFound}";
        public const string REST_MODULE_UNINSTALL = "module/uninstall?csvPackageNames={csvPackageNames}";
        #endregion

        #region Constructor
        public ModuleAdminClient() { }
        public ModuleAdminClient(string targetDnnRootUrl, string userName, string password) : base(targetDnnRootUrl, userName, password) { }
        #endregion

        #region REST for Module
        public bool ModuleInstall(bool deleteModuleFirstIfFound, params string[] modulesFilePath)
        {
            var request = REST_CreateRequest(REST_MODULE_INSTALL, Method.PUT, new Dictionary<string, string> { { "deleteModuleFirstIfFound", deleteModuleFirstIfFound.ToString() } });
            // add files to upload
            foreach (var item in modulesFilePath)
            { if (!string.IsNullOrWhiteSpace(item)) { request.AddFile(Path.GetFileName(item), item); } }

            // execute the request
            var response = REST_Execute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public bool ModuleUninstall(params string[] moduleNames)
        {
            var modules = string.Join(",", moduleNames);
            var response = REST_Execute(REST_MODULE_UNINSTALL, Method.DELETE, new Dictionary<string, string> { { "csvPackageNames", modules } });
            return response.StatusCode == HttpStatusCode.OK;
        }

        public string ModuleGet(string filterPattern = "", bool builtIn = false)
        {
            var response = REST_Execute(REST_MODULE_GET, new Dictionary<string, string> { { "filterPattern", filterPattern }, { "builtIn", builtIn.ToString() } });
            return response.Content;
        }

        public string ModuleGetDesktop(string filterPattern = "")
        {
            var response = REST_Execute(REST_MODULE_GET_DESKTOP, new Dictionary<string, string> { { "filterPattern", filterPattern } });
            return response.Content;
        }


        #endregion
    }
}
