using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Build.DotNetNuke.Deployer.Client
{
    public class InstallFolderAdminClient : AdminBaseClient
    {
        #region Constants
        public const string REST_INSTALL_FOLDER_GET = "installfolder/get?packageType={packageType}";
        public const string REST_INSTALL_FOLDER_COUNT = "installfolder/count?packageType={packageType}";
        public const string REST_INSTALL_FOLDER_SAVE = "installfolder/save";
        public const string REST_INSTALL_FOLDER_DELETE = "installfolder/delete?packageType={packageType}&csvPackageNames={csvPackageNames}";
        public const string REST_INSTALL_FOLDER_CLEAR = "installfolder/clear?packageType={packageType}";
        #endregion

        #region Constructor
        public InstallFolderAdminClient() : base() { }
        public InstallFolderAdminClient(string targetDnnRootUrl, string userName, string password) : base(targetDnnRootUrl, userName, password) { }
        #endregion

        #region REST for Module
        public bool Save(params string[] modulesFilePath)
        {
            var request = REST_CreateRequest(REST_INSTALL_FOLDER_SAVE, Method.PUT);

            List<string> modulesToUpload = new List<string>();

            // check paths passed in whether a file or a folder
            foreach (var item in modulesFilePath)
            {
                if (string.IsNullOrWhiteSpace(item)) { continue; }

                // [2015-08-14] allow specifying a folder with all files within
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    var files = Directory.GetFiles(item, "*.zip", SearchOption.AllDirectories);
                    foreach (string file in files) { modulesToUpload.Add(file); }
                }
                else { modulesToUpload.Add(Path.GetFullPath(item)); }
            }

            // add files to upload
            foreach (var item in modulesToUpload) { request.AddFile(Path.GetFileName(item), item); }

            // execute the request
            var response = REST_Execute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public string Get(string packageType = "")
        {
            var response = REST_Execute(REST_INSTALL_FOLDER_GET, new Dictionary<string, string> { { "packageType", packageType } });
            return response.Content;
        }

        public int Count(string packageType = "")
        {
            var response = REST_Execute(REST_INSTALL_FOLDER_COUNT, new Dictionary<string, string> { { "packageType", packageType } });
            return Convert.ToInt32(response.Content);
        }

        public int Delete(string packageType, params string[] moduleNames)
        {
            var modules = string.Join(",", moduleNames);
            var response = REST_Execute(REST_INSTALL_FOLDER_DELETE, Method.DELETE, new Dictionary<string, string> { { "packageType", packageType }, { "csvPackageNames", modules } });
            return Convert.ToInt32(response.Content);
        }

        public string Clear(string packageType = "")
        {
            var response = REST_Execute(REST_INSTALL_FOLDER_CLEAR, Method.DELETE, new Dictionary<string, string> { { "packageType", packageType } });
            return response.Content;
        }
        #endregion
    }
}
