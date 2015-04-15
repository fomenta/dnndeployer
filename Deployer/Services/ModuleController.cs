using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Build.DotNetNuke.Deployer.Services
{
    [RequireHost]
    public class ModuleController : AdminBaseController
    {
        [HttpGet]
        public HttpResponseMessage Get(string filterPattern = "", bool builtIn = false)
        {
            var packages = from p in PackageController.Instance
                                .GetExtensionPackages(Null.NullInteger,
                                    p =>
                                    p.PackageType == PackageTypes.Module &&
                                            (builtIn || (p.Organization != "DNN Corp." && p.Organization != "DotNetNuke Corporation")) &&
                                            (string.IsNullOrWhiteSpace(filterPattern) || Regex.IsMatch(p.Name, filterPattern, RegexOptions.IgnoreCase))
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

        [HttpGet]
        public HttpResponseMessage GetDesktop(string filterPattern = "")
        {
            var modules = from d in DesktopModuleController.GetDesktopModules(UserInfo.PortalID)
                          where (string.IsNullOrWhiteSpace(filterPattern) || Regex.IsMatch(d.Value.ModuleName, filterPattern, RegexOptions.IgnoreCase))
                          let moduleDefinition = (from md in ModuleDefinitionController.GetModuleDefinitions()
                                                  where md.Value.DesktopModuleID == d.Value.DesktopModuleID
                                                  select md.Value).FirstOrDefault()
                          select new
                          {
                              ModuleID = d.Value.ModuleID,
                              ModuleDefID = moduleDefinition.ModuleDefID,
                              ModuleName = d.Value.ModuleName,
                              FriendlyName = d.Value.FriendlyName,
                              ModuleDefinition_FriendlyName = moduleDefinition.FriendlyName,
                              DefinitionName = moduleDefinition.DefinitionName,
                              DesktopModuleID = d.Value.DesktopModuleID,
                              FolderName = d.Value.FolderName,
                              LastModifiedOnDate = d.Value.LastModifiedOnDate,
                              IsAdmin = d.Value.IsAdmin,
                              TabID = d.Value.TabID,
                          };
            return Request.CreateResponse(HttpStatusCode.OK, modules, new MediaTypeHeaderValue("text/json"));
        }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Install(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.Module, HttpContextSource.Current.Request.Files, deleteModuleFirstIfFound); }

        [HttpDelete]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage Uninstall(string csvPackageNames)
        {
            if (string.IsNullOrEmpty(csvPackageNames)) { return Request.CreateResponse(HttpStatusCode.BadRequest); }
            var packageNames = csvPackageNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return UninstallExtensions(PackageTypes.Module, packageNames);
        }
    }
}