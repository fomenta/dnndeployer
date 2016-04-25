using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Build.DotNetNuke.Deployer.Client
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
        public ModuleAdminClient() : base() { }
        public ModuleAdminClient(string targetDnnRootUrl, string userName, string password) : base(targetDnnRootUrl, userName, password) { }
        #endregion

        #region REST for Module
        public bool ModuleInstall(bool deleteModuleFirstIfFound, params string[] modulesFilePath)
        {
            var request = REST_CreateRequest(REST_MODULE_INSTALL, Method.PUT, new Dictionary<string, string> { { "deleteModuleFirstIfFound", deleteModuleFirstIfFound.ToString() } });

            List<string> modulesToInstall = new List<string>();

            // check paths passed in whether a file or a folder
            foreach (var item in modulesFilePath)
            {
                if (string.IsNullOrWhiteSpace(item)) { continue; }

                // [2015-08-14] allow specifying a folder with all files within
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    var files = Directory.GetFiles(item, "*.zip", SearchOption.AllDirectories);
                    foreach (string file in files) { modulesToInstall.Add(file); }
                }
                else { modulesToInstall.Add(Path.GetFullPath(item)); }
            }

            // add files to upload
            foreach (var item in modulesToInstall) { request.AddFile(Path.GetFileName(item), item); }

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
