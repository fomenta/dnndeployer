﻿using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using d = System.Diagnostics;

namespace Build.DotNetNuke.Deployer.Services
{
    /// <summary>
    /// http://goo.gl/QNAHMg
    /// </summary>
    public class AdminBaseController : DnnApiController
    {
        #region Constructor
        protected string InstallFolder { get; private set; }

        public AdminBaseController()
        {
            InstallFolder = Path.Combine(Globals.ApplicationMapPath, "Install");
        }
        #endregion

        #region REST verbs
        /// <summary>
        /// Default verb for controller
        /// </summary>
        /// <returns>Gets the assembly version number and the DNN version number</returns>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var dnnAssembly = typeof(DnnApiController).Assembly;

            var result = string.Format("{0}: {2} [{1}]; {3}: {4}",
                                        thisAssembly.GetName().Name, thisAssembly.GetName().Version.ToString(), d.FileVersionInfo.GetVersionInfo(thisAssembly.Location).FileVersion,
                                        dnnAssembly.GetName().Name, d.FileVersionInfo.GetVersionInfo(dnnAssembly.Location).FileVersion);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage InstallAuthSystems(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.AuthSystem, HttpContextSource.Current.Request.Files); }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage InstallLanguages(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.Language, HttpContextSource.Current.Request.Files); }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage InstallSkins(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.Skin, HttpContextSource.Current.Request.Files); }

        [HttpPut]
        //[IFrameSupportedValidateAntiForgeryToken]
        public HttpResponseMessage InstallContainers(bool deleteModuleFirstIfFound = false) { return InstallExtensions(PackageTypes.Container, HttpContextSource.Current.Request.Files); }
        #endregion

        #region Protected Helpers
        protected HttpResponseMessage SaveExtensions(HttpFileCollectionBase Files)
        {
            var context = HttpContextSource.Current;
            List<DeployResponseItem> filesProcessed = new List<DeployResponseItem>();

            if (context.Request.Files.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent,
                                      new DeployResponse { ErrorMessage = "No files were uploaded" },
                                      new MediaTypeHeaderValue("text/json"));
            }

            //Get current Script time-out
            int scriptTimeout = context.Server.ScriptTimeout;
            bool callSuccess = true;
            try
            {
                //Set Script timeout to MAX value
                context.Server.ScriptTimeout = int.MaxValue;

                for (var i = 0; i < Files.Count; i++)
                {
                    var file = context.Request.Files[i];
                    var uploadResponse = SaveExtension(file);

                    var success = string.IsNullOrEmpty(uploadResponse.ErrorMessage);
                    filesProcessed.Add(new DeployResponseItem { Extension = file.FileName, Success = uploadResponse.Success, ErrorMessage = uploadResponse.ErrorMessage });
                }

                return Request.CreateResponse(callSuccess ? HttpStatusCode.OK : HttpStatusCode.Conflict,
                                        new DeployResponse { Success = callSuccess, AffectedItems = filesProcessed.Count, Files = filesProcessed },
                                        new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                        new DeployResponse { Success = false, ErrorMessage = ex.ToString(), AffectedItems = filesProcessed.Count, Files = filesProcessed },
                                        new MediaTypeHeaderValue("text/json"));
            }
            finally
            {
                //restore Script timeout
                context.Server.ScriptTimeout = scriptTimeout;
            }
        }


        protected HttpResponseMessage InstallExtensions(string packageType, HttpFileCollectionBase Files, bool deleteModuleFirstIfFound = false)
        {
            var context = HttpContextSource.Current;
            List<DeployResponseItem> filesProcessed = new List<DeployResponseItem>();

            if (context.Request.Files.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent,
                                      new DeployResponse { ErrorMessage = "No files were uploaded" },
                                      new MediaTypeHeaderValue("text/json"));
            }

            //Get current Script time-out
            var scriptTimeout = context.Server.ScriptTimeout;
            var callSuccess = true;
            try
            {
                //Set Script timeout to MAX value
                context.Server.ScriptTimeout = int.MaxValue;

                string InstallPath = Path.Combine(Globals.ApplicationMapPath, "Install", packageType);
                if (!Directory.Exists(InstallPath)) { Directory.CreateDirectory(InstallPath); }

                string errorMessage = "";
                for (var i = 0; i < Files.Count; i++)
                {
                    var file = context.Request.Files[i];
                    var uploadResponse = SaveExtension(file);
                    if (!string.IsNullOrEmpty(uploadResponse.ErrorMessage))
                    {
                        filesProcessed.Add(new DeployResponseItem { Extension = file.FileName, Success = false, ErrorMessage = uploadResponse.ErrorMessage });
                        continue;
                    }

                    // uninstall old one if found (when requested)
                    if (deleteModuleFirstIfFound)
                    {
                        PackageInfo package = GetPackage(packageType, uploadResponse.PackageName);
                        if (package != null)
                        {
                            var response = UninstallExtensions(packageType, new[] { uploadResponse.PackageName });
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                errorMessage = string.Format("WARNING: Could not uninstalling module '{0}' first. Proceeding to install on top of existing module.", uploadResponse.PackageName);
                            }
                        }
                    }

                    // install
                    var success = Upgrade.InstallPackage(uploadResponse.FullName, packageType, writeFeedback: false);
                    if (!success)
                    {
                        errorMessage = "Error installing module. Check DotNetNuke Log for more details on error. It is possible a more recent version of the module was found installed. Use '--force' parameter to overwrite!";
                        callSuccess = false;
                    }
                    filesProcessed.Add(new DeployResponseItem { Extension = file.FileName, Success = success, ErrorMessage = errorMessage });
                }

                return Request.CreateResponse(callSuccess ? HttpStatusCode.OK : HttpStatusCode.Conflict,
                                        new DeployResponse { Success = callSuccess, AffectedItems = filesProcessed.Count, Files = filesProcessed },
                                        new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                        new DeployResponse { Success = false, ErrorMessage = ex.ToString(), AffectedItems = filesProcessed.Count, Files = filesProcessed },
                                        new MediaTypeHeaderValue("text/json"));
            }
            finally
            {
                //restore Script timeout
                context.Server.ScriptTimeout = scriptTimeout;
            }
        }

        private PackageInfo ParsePackage(string fullPath, string InstallPath)
        {
            Dictionary<string, PackageInfo> packages = new Dictionary<string, PackageInfo>();
            List<string> invalidPackages = new List<string>();
            PackageController.ParsePackage(fullPath, InstallPath, packages, invalidPackages);

            if (packages.Count == 0) { return null; }
            else { return packages[packages.Keys.First()]; }
        }

        protected HttpResponseMessage UninstallExtensions(string packageType, string[] packageNames)
        {
            var context = HttpContextSource.Current;
            List<DeployResponseItem> extensionsDeleted = new List<DeployResponseItem>();

            //Get current Script time-out
            int scriptTimeout = context.Server.ScriptTimeout;
            bool callSuccess = true;
            try
            {
                //Set Script timeout to MAX value
                context.Server.ScriptTimeout = int.MaxValue;

                foreach (var packageName in packageNames)
                {
                    PackageInfo package = GetPackage(packageType, packageName);
                    if (package == null)
                    {
                        callSuccess = true;
                        extensionsDeleted.Add(new DeployResponseItem { Extension = packageName, Success = true, NotFound = true });
                    }
                    else
                    {
                        var installer = new Installer(package, Globals.ApplicationMapPath);
                        var success = installer.UnInstall(deleteFiles: true);
                        if (!success) { callSuccess = false; }
                        extensionsDeleted.Add(new DeployResponseItem { Extension = package.Name, Success = success });
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
            finally
            {
                //restore Script timeout
                context.Server.ScriptTimeout = scriptTimeout;
            }
        }

        protected PackageInfo GetPackage(string packageType, string packageName)
        {
            return PackageController.Instance.GetExtensionPackage(Null.NullInteger,
                            p => p.PackageType == packageType &&
                            p.Name.Equals(packageName.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Find location for folder on which page templates are stored (when exporting pages/tabs)
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTemplateFolder()
        {
            var templateFolder = FolderManager.Instance.GetFolders(UserInfo, permissions: "BROWSE, ADD")
                            .Where(f => f.FolderPath == "Templates/")
                            .Select(f => f.FolderPath)
                            .First();
            // remove final slash (usually found as "Templates/")
            if (templateFolder.EndsWith("/")) { templateFolder = templateFolder.Substring(0, templateFolder.Length - 1); }
            // return full path
            return Path.Combine(PortalSettings.HomeDirectoryMapPath, templateFolder);
        }

        protected virtual List<string> AllowedExtensionsLowerCase { get { return new List<string> { ".zip" }; } }

        protected virtual bool IsAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName); // it includes the dot ('.')

            //regex matches a dot followed by 1 or more chars followed by a semi-colon
            //regex is meant to block files like "foo.asp;.png" which can take advantage
            //of a vulnerability in IIS6 which treasts such files as .asp, not .png
            return !string.IsNullOrEmpty(extension) && AllowedExtensionsLowerCase.Contains(extension.ToLower());
        }

        protected void FileDelete(string fullName)
        {
            var fi = new System.IO.FileInfo(fullName);
            if (fi.Exists)
            {
                if (fi.IsReadOnly) { fi.IsReadOnly = false; }
                File.Delete(fullName);
            }
        }
        protected string GetModuleSubfolderName(string packageType)
        {
            // [2016-05-23] folder name differs from package type
            return packageType == "Auth_System" ? "AuthSystem" : packageType;
        }
        #endregion

        #region return Response*
        protected HttpResponseMessage ResponseNotFound(string errorMessageResourceKey, params object[] args)
        {
            return Request.CreateResponse(HttpStatusCode.NotFound,
                  //new DeployResponse { ErrorMessage = string.Format(errorMessageResourceKey, args) },
                  string.Format(errorMessageResourceKey, args),
                  new MediaTypeHeaderValue("text/json"));
        }

        #endregion

        #region Private
        private UploadResponse SaveExtension(HttpPostedFileBase file, bool deleteOtherVersions = true)
        {
            var response = new UploadResponse();

            response.PackageType = PackageTypes.Module;    // default location
            response.InstallDir = Path.Combine(Globals.ApplicationMapPath, "Install", GetModuleSubfolderName(response.PackageType));
            if (!Directory.Exists(response.InstallDir)) { Directory.CreateDirectory(response.InstallDir); }

            if (file == null) { return null; }
            var fileName = Path.GetFileName(file.FileName);
            const string PATTERN_BASE_FILENAME = @"^(.+)_(\d+\.\d+\.\d+).*$";
            var baseFile = Regex.Replace(fileName, PATTERN_BASE_FILENAME, "$1");
            if (string.IsNullOrEmpty(baseFile)) { baseFile = Path.GetFileNameWithoutExtension(fileName); }

            if (!IsAllowedExtension(fileName))
            {
                response.ErrorMessage = string.Format("Unsupported file extension: '{0}'", fileName);
                return response;
            }

            response.FullName = Path.Combine(response.InstallDir, fileName);
            FileDelete(response.FullName);

            // delete other versions pending to be installed for the same package
            if (deleteOtherVersions)
            {
                var otherVersions = Directory.GetFiles(response.InstallDir, string.Format("{0}*.zip", baseFile));
                if (otherVersions != null)
                {
                    foreach (var item in otherVersions) { FileDelete(item); }
                }
            }

            // save new package
            file.SaveAs(response.FullName);

            // validate
            PackageInfo parsedPackage = ParsePackage(response.FullName, response.InstallDir);
            if (parsedPackage == null)
            {
                response.ErrorMessage = string.Format("Invalid package: '{0}'", fileName);
                return response;
            }

            response.PackageName = parsedPackage.Name;

            // place package in the right place
            var newPackageType = parsedPackage.PackageType;
            if (response.PackageType != newPackageType)
            {
                response.PackageType = newPackageType;
                var newInstallPath = Path.Combine(Globals.ApplicationMapPath, "Install", GetModuleSubfolderName(newPackageType));
                if (!Directory.Exists(newInstallPath)) { Directory.CreateDirectory(newInstallPath); }

                var newFullPath = Path.Combine(newInstallPath, fileName);

                var targetFileInfo = new System.IO.FileInfo(newFullPath);
                if (targetFileInfo.Exists)
                {
                    if (targetFileInfo.IsReadOnly) { targetFileInfo.IsReadOnly = false; }
                    File.Delete(newFullPath);
                }

                File.Move(response.FullName, newFullPath);

                response.InstallDir = newInstallPath;
                response.FullName = newFullPath;
            }

            response.Success = true;
            return response;
        }
        #endregion
    }
}