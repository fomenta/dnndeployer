using Build.DotNetNuke.Deployer.Library;
using Build.Extensions.DotNetNuke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;

namespace Build.Extensions.Tests
{
    public class BaseUnitTest
    {
        public const int WORKFLOW_RUN_TIMEOUT_SEC = 10;

        #region Fields
        private static string _BaseDirectory;
        private static List<string> ExcludeDirs = new List<string> { "bin", "Debug", "Release" };
        #endregion

        #region Methods
        public string CopyToTemporaryFile(string filePathToCopy)
        {
            string destFileName = Path.GetTempFileName().Replace(".tmp", ".dnn");
            File.Copy(filePathToCopy, destFileName);
            return destFileName;
        }

        public string XmlGetValue(string xmlFilePath, string xpathExpression, string xmlNamespace = null, string xmlNamespaceAlias = null)
        {
            // Load the document and set the root element.
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            XmlNode root = doc.DocumentElement;

            // Add the namespace.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            if (!string.IsNullOrEmpty(xmlNamespace)) { nsmgr.AddNamespace(xmlNamespaceAlias, xmlNamespace); }

            // Select and display the first node 
            XmlNode node = root.SelectSingleNode(xpathExpression, nsmgr);
            return node.InnerText;
        }
        #endregion

        #region Properties
        public readonly static string DNN_URL = ConfigurationManager.AppSettings["DotNetNuke.Url"];
        public readonly static string DNN_USERNAME = ConfigurationManager.AppSettings["DotNetNuke.UserName"];
        public readonly static string DNN_PASSWORD = ConfigurationManager.AppSettings["DotNetNuke.Password"];

        public TestContext TestContext { get; set; }

        public static string BaseDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(_BaseDirectory)) { return _BaseDirectory; }

                _BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //var i = _BaseDirectory.IndexOf(@"\test\");
                //if (i > -1) { _BaseDirectory = _BaseDirectory.Substring(0, i); }

                if (_BaseDirectory.EndsWith(@"\")) { _BaseDirectory = _BaseDirectory.Substring(0, _BaseDirectory.Length - 1); }
                while (ExcludeDirs.Contains(Path.GetFileNameWithoutExtension(_BaseDirectory)))
                { _BaseDirectory = Path.GetDirectoryName(_BaseDirectory); }

                return _BaseDirectory;
            }
        }
        #endregion

        #region Paths
        public static string AppData { get { return Path.Combine(BaseDirectory, "App_Data"); } }
        #endregion

        protected void CheckAndDisplayResponse(AdminBaseClient client)
        {
            TestContext.WriteLine("LastRequestUrl: {0}", client.LastRequestUrl);
            DisplayResponse(client.LastResponse);
        }

        protected void DisplayResponse(IRestResponse response)
        {
            Assert.IsNotNull(response, "Response");
            TestContext.WriteLine("");
            TestContext.WriteLine("StatusCode: [{0}] {1}", (int)response.StatusCode, response.StatusCode);
            TestContext.WriteLine("StatusDescription: {0}", response.StatusDescription);
            TestContext.WriteLine("[Response Content]");
            TestContext.WriteLine("{0}", FormatJSON(response.Content));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        public string FormatJSON(string json)
        {
            try
            { return JToken.Parse(json).ToString(); }
            catch (JsonReaderException)
            { return json; }
        }

        #region Dnn Module Helpers
        public bool ModuleInstall(bool deleteModuleFirstIfFound, params string[] modulesFilePath)
        {
            TestContext.WriteLine("[ModuleInstall: {0}]", modulesFilePath == null ? "" : string.Join(", ", modulesFilePath));

            var client = new DeployerUnsecuredClient(DNN_URL);
            var success = client.ModuleInstall(deleteModuleFirstIfFound, modulesFilePath);
            TestContext.WriteLine("LastRequestUrl: {0}", client.LastRequestUrl);

            TestContext.WriteLine("[Response]");
            CheckAndDisplayResponse(client);
            Assert.AreEqual(true, success, "Success");

            // check whether they were installed
            TestContext.WriteLine("[Verify Module Were Installed]");
            foreach (var item in modulesFilePath)
            {
                var fileName = Path.GetFileName(item);
                ModuleGet(fileName.Substring(0, Math.Min(4, fileName.Length)));
            }

            return success;
        }

        public bool ModuleUninstall(params string[] moduleNames)
        {
            TestContext.WriteLine("[ModuleUninstall: {0}]", moduleNames == null ? "" : string.Join(", ", moduleNames));
            var client = new DeployerUnsecuredClient(DNN_URL);
            var success = client.ModuleUninstall(moduleNames);
            TestContext.WriteLine("LastRequestUrl: {0}", client.LastRequestUrl);

            // display
            TestContext.WriteLine("[ModuleUninstall]");
            CheckAndDisplayResponse(client);
            Assert.AreEqual(true, success, "Success");

            // check whether they were installed
            TestContext.WriteLine("[Verify Module Were Uninstalled]");
            foreach (var item in moduleNames) { ModuleGet(item); }

            return success;
        }

        public void ModuleGet(string filterPattern, bool builtIn = false)
        {
            var client = new DeployerUnsecuredClient(DNN_URL);
            var modules = client.ModuleGet(filterPattern, builtIn);
            CheckAndDisplayResponse(client);
        }
        #endregion

        #region Dnn Module Secured Helpers
        public bool ModuleAdminInstall(bool deleteModuleFirstIfFound, params string[] modulesFilePath)
        {
            var client = new ModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var success = client.ModuleInstall(deleteModuleFirstIfFound, modulesFilePath);
            TestContext.WriteLine("LastRequestUrl: {0}", client.LastRequestUrl);

            TestContext.WriteLine("[Secured:ModuleInstall]");
            CheckAndDisplayResponse(client);
            Assert.AreEqual(true, success, "Success");

            // check whether they were installed
            TestContext.WriteLine("[Verify Module Were Installed]");
            foreach (var item in modulesFilePath)
            {
                var fileName = Path.GetFileName(item);
                ModuleSecuredList(fileName.Substring(0, Math.Min(4, fileName.Length)));
            }

            return success;
        }

        public bool ModuleAdminUninstall(params string[] moduleNames)
        {
            var client = new ModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var success = client.ModuleUninstall(moduleNames);
            TestContext.WriteLine("LastRequestUrl: {0}", client.LastRequestUrl);

            // display
            TestContext.WriteLine("[Secured:ModuleUninstall]");
            CheckAndDisplayResponse(client);
            Assert.AreEqual(true, success, "Success");

            // check whether they were installed
            TestContext.WriteLine("[Verify Module Were Uninstalled]");
            foreach (var item in moduleNames) { ModuleSecuredList(item); }

            return success;
        }

        public void ModuleSecuredList(string filterPattern, bool builtIn = false)
        {
            var client = new ModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var modules = client.ModuleGet(filterPattern, builtIn);
            CheckAndDisplayResponse(client);
        }


        public void ModuleSecuredListDesktop(string filterPattern)
        {
            var client = new ModuleAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var modules = client.ModuleGetDesktop(filterPattern);
            CheckAndDisplayResponse(client);
        }
        #endregion

        #region Dnn Page Helpers
        public bool PageDelete(PageGetRequest page, bool deleteDescendants = false)
        {
            var client = new PageAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var deleted = client.PageDelete(page, deleteDescendants);
            CheckAndDisplayResponse(client);
            return deleted;
        }

        public int PageImport(string pageTemplateFilePath,
                              string parentPagePath = "", string beforePagePath = "", string afterPagePath = "",
                              string parentPageFullName = null, string beforePageFullName = null, string afterPageFullName = null)
        {
            var client = new PageAdminClient(DNN_URL, DNN_USERNAME, DNN_PASSWORD);
            var pagesImported = client.PageImport(pageTemplateFilePath, string.Empty, null,
                                                  parentPagePath, beforePagePath, afterPagePath,
                                                  parentPageFullName, beforePageFullName, afterPageFullName);
            CheckAndDisplayResponse(client);
            Assert.IsTrue(pagesImported > 0, "Expecting at least one page imported. pagesImported: {0}", pagesImported);

            return pagesImported;
        }
        #endregion


    }
}
