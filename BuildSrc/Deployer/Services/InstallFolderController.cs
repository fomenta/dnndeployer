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
            string installFolder;
            if ((response = ResolveInstallFolder(out installFolder, packageType)) != null) { return response; }

            var files = Directory.GetFiles(installFolder, "*.zip", SearchOption.AllDirectories).Select(x => x.Replace(installFolder + @"\", "")).ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, files, new MediaTypeHeaderValue("text/json"));
        }

        private HttpResponseMessage ResolveInstallFolder(out string installFolder, string packageType)
        {
            installFolder = Path.Combine(Globals.ApplicationMapPath, "Install");
            if (!string.IsNullOrEmpty(packageType))
            {
                if (PackageTypes.IsInvalid(packageType)) { return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentOutOfRangeException("packageType", string.Format("Invalid value: '{0}'", packageType))); }
                installFolder = Path.Combine(installFolder, packageType);
            }
            return null;
        }

        [HttpGet]
        public HttpResponseMessage Count(string packageType = "")
        {
            string installFolder; HttpResponseMessage response;
            if ((response = ResolveInstallFolder(out installFolder, packageType)) != null) { return response; }

            var fileCount = Directory.GetFiles(installFolder, "*.zip", SearchOption.AllDirectories).Select(x => x.Replace(installFolder + "/", "")).Count();
            return Request.CreateResponse<int>(HttpStatusCode.OK, fileCount, new MediaTypeHeaderValue("text/json"));
        }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Save() { return SaveExtensions(HttpContextSource.Current.Request.Files); }

        [HttpPut]
        public HttpResponseMessage InstallResources()
        {
            //install new resources(s)
            var packages = ds.Upgrade.Upgrade.GetInstallPackages();
            foreach (var package in packages)
            {
                ds.Upgrade.Upgrade.InstallPackage(package.Key, package.Value.PackageType, writeFeedback: false);
            }

            return Request.CreateResponse<bool>(HttpStatusCode.OK, true, new MediaTypeHeaderValue("text/json"));
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
        public HttpResponseMessage Clear(string packageType = "") { return RemoveExtensions(PackageTypes.Module, "*"); }


        #region Private
        private HttpResponseMessage RemoveExtensions(string packageType, params string[] packageNames)
        {
            string installFolder; HttpResponseMessage response;
            if ((response = ResolveInstallFolder(out installFolder, packageType)) != null) { return response; }

            List<DeployResponseItem> extensionsDeleted = new List<DeployResponseItem>();

            bool callSuccess = true;
            try
            {
                foreach (var packageName in packageNames)
                {
                    var files = Directory.GetFiles(installFolder, "*" + packageName + "*.zip", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        var fi = new FileInfo(file);
                        if (fi.IsReadOnly) { fi.IsReadOnly = false; }
                        fi.Delete();

                        extensionsDeleted.Add(new DeployResponseItem { Extension = file, Success = true });
                    }

                    if (files.Length == 0)
                    {
                        extensionsDeleted.Add(new DeployResponseItem { Extension = packageName, Success = false, NotFound = true });
                    }
                }

                return Request.CreateResponse(callSuccess ? HttpStatusCode.OK : HttpStatusCode.Conflict,
                                        new DeployResponse { Success = callSuccess, AffectedItems = extensionsDeleted.Count, Files = extensionsDeleted },
                                        new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                        new DeployResponse { Success = false, ErrorMessage = ex.ToString(), AffectedItems = extensionsDeleted.Count, Files = extensionsDeleted },
                                        new MediaTypeHeaderValue("text/json"));
            }
        }
        #endregion
    }
}