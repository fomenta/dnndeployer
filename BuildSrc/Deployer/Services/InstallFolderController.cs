using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Web.Api;
using ds = DotNetNuke.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Build.DotNetNuke.Deployer.Services
{
    [RequireHost]
    public class InstallFolderController : AdminBaseController
    {
        [HttpGet]
        public HttpResponseMessage Get(string packageType = "")
        {
            HttpResponseMessage response;
            var files = GetFiles(out response, packageType);
            if (response != null) { return response; }

            var filesRelativePath = files.Select(x => x.Replace(InstallFolder + @"\", "")).ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, filesRelativePath, new MediaTypeHeaderValue("text/json"));
        }


        [HttpGet]
        public HttpResponseMessage Count(string packageType = "")
        {
            HttpResponseMessage response;
            var files = GetFiles(out response, packageType);
            if (response != null) { return response; }

            return Request.CreateResponse<int>(HttpStatusCode.OK, files.Count, new MediaTypeHeaderValue("text/json"));
        }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Save() { return SaveExtensions(HttpContextSource.Current.Request.Files); }

        [HttpPut]
        public HttpResponseMessage InstallResources()
        {
            var context = HttpContextSource.Current;
            List<DeployResponseItem> filesProcessed = new List<DeployResponseItem>();

            var scriptTimeout = context.Server.ScriptTimeout;
            var callSuccess = true;
            try
            {
                //install new resources(s)
                var packages = ds.Upgrade.Upgrade.GetInstallPackages();
                foreach (var package in packages)
                {
                    var success = ds.Upgrade.Upgrade.InstallPackage(package.Key, package.Value.PackageType, writeFeedback: false);
                    filesProcessed.Add(new DeployResponseItem { Extension = package.Key, Success = success, ErrorMessage = success ? null : "Check error log" });
                    if (!success) { callSuccess = false; }
                }

                if (packages.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent,
                                          new DeployResponse { ErrorMessage = "No files to installed" },
                                          new MediaTypeHeaderValue("text/json"));
                }


                return Request.CreateResponse(callSuccess ? HttpStatusCode.OK : HttpStatusCode.Conflict,
                                        new DeployResponse { Success = callSuccess, AffectedItems = filesProcessed.Count, Files = filesProcessed },
                                        new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                        new DeployResponse
                                        {
                                            Success = false,
                                            ErrorMessage = ex.ToString(),
                                            AffectedItems = filesProcessed.Count,
                                            Files = filesProcessed
                                        },
                                        new MediaTypeHeaderValue("text/json"));
            }
            finally
            {
                //restore Script timeout
                context.Server.ScriptTimeout = scriptTimeout;
            }
        }


        [HttpDelete]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Delete(string packageType, string csvPackageNames)
        {
            if (string.IsNullOrEmpty(csvPackageNames)) { return Request.CreateResponse(HttpStatusCode.BadRequest); }
            var packageNames = csvPackageNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return RemoveExtensions(PackageTypes.Module, packageNames);
        }


        [HttpDelete]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Clear(string packageType = "") { return RemoveExtensions(packageType, "*"); }


        #region Private
        private HttpResponseMessage RemoveExtensions(string packageType, params string[] packageNames)
        {
            HttpResponseMessage response;
            var files = GetFiles(out response, packageType, packageNames);
            if (response != null) { return response; }

            List<DeployResponseItem> extensionsDeleted = new List<DeployResponseItem>();

            bool callSuccess = true;
            try
            {
                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.IsReadOnly) { fi.IsReadOnly = false; }
                    fi.Delete();

                    extensionsDeleted.Add(new DeployResponseItem { Extension = file, Success = true });
                }

                if (files.Count == 0)
                {
                    extensionsDeleted.Add(new DeployResponseItem { Extension = string.Join(", ", packageNames), Success = false, NotFound = true });
                }

                return Request.CreateResponse(callSuccess ? HttpStatusCode.OK : HttpStatusCode.Conflict,
                                        new DeployResponse { Success = callSuccess, AffectedItems = files.Count, Files = extensionsDeleted },
                                        new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                        new DeployResponse { Success = false, ErrorMessage = ex.ToString(), AffectedItems = extensionsDeleted.Count, Files = extensionsDeleted },
                                        new MediaTypeHeaderValue("text/json"));
            }
        }


        private HttpResponseMessage ResolveInstallFolder(out string[] installSubfolders, string packageType)
        {
            if (!string.IsNullOrWhiteSpace(packageType))
            {
                installSubfolders = null;
                if (PackageTypes.IsInvalid(packageType)) { return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentOutOfRangeException("packageType", string.Format("Invalid value: '{0}'", packageType))); }
                installSubfolders = new[] { Path.Combine(InstallFolder, packageType) };
            }
            else
            {
                installSubfolders = PackageTypes.Subfolders.Select(subfolder => Path.Combine(InstallFolder, subfolder)).ToArray();
            }
            return null;
        }


        private List<string> GetFiles(out HttpResponseMessage response, string packageType, params string[] packageNames)
        {
            string[] installSubfolders;
            if ((response = ResolveInstallFolder(out installSubfolders, packageType)) != null) { return null; }

            if (packageNames == null || packageNames.Length == 0) { packageNames = new[] { "*" }; }

            var files = new List<string>();
            foreach (var installSubfolder in installSubfolders)
            {
                foreach (var packageName in packageNames)
                {
                    files.AddRange(Directory.GetFiles(installSubfolder, "*" + packageName + "*.zip", SearchOption.TopDirectoryOnly));
                    files.AddRange(Directory.GetFiles(installSubfolder, "*" + packageName + "*.resources", SearchOption.TopDirectoryOnly));
                }
            }

            return files;
        }
        #endregion
    }
}