using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Web.Api.Internal;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Build.DotNetNuke.Deployer.Services
{
    [AllowAnonymous]
    [Obsolete("ModuleController is no longer secure. Use ModuleController instead.")]
    internal class ModuleUnsecuredController : AdminBaseController
    {
        [Obsolete("Use ModuleController.Get() instead")]
        [HttpGet]
        public HttpResponseMessage Get(string filterPattern = "", bool builtIn = false)
        {
            var packages = from p in PackageController.Instance
                                .GetExtensionPackages(Null.NullInteger,
                                    p =>
                                    p.PackageType == PackageTypes.Module &&
                                            (builtIn || (p.Organization != "DNN Corp." && p.Organization != "DotNetNuke Corporation")) &&
                                            (string.IsNullOrEmpty(filterPattern) || Regex.IsMatch(p.Name, filterPattern, RegexOptions.IgnoreCase))
                                    )
                           select new
                           {
                               PackageID = p.PackageID,
                               Name = p.Name,
                               FriendlyName = p.FriendlyName,
                               FolderName = p.FolderName,
                               Version = string.Format("{0}.{1}.{2}.{3}", p.Version.Major, p.Version.Minor, p.Version.Build, p.Version.Revision),
                               LastModifiedOnDate = p.LastModifiedOnDate
                           };
            return Request.CreateResponse(HttpStatusCode.OK, packages, new MediaTypeHeaderValue("text/json"));
        }

        [Obsolete("Use ModuleController.Install() instead")]
        [HttpPut]
        [IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Install(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.Module, HttpContextSource.Current.Request.Files, deleteModuleFirstIfFound); }

        [Obsolete("Use ModuleController.Uninstall() instead")]
        [HttpDelete]
        [IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Uninstall(string csvPackageNames)
        {
            if (string.IsNullOrEmpty(csvPackageNames)) { return Request.CreateResponse(HttpStatusCode.BadRequest); }
            var packageNames = csvPackageNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return UninstallExtensions(PackageTypes.Module, packageNames);
        }
    }
}