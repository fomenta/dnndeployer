using Build.DotNetNuke.Deployer.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Build.Extensions.Tests.DotNetNuke
{
    [TestClass]
    public class InstallFolderAdminClientTests : BaseUnitTest<InstallFolderAdminClient>
    {
        [TestMethod]
        public void InstallFolder_1_1_Get()
        {
            var list = client.Get();
            TestContext.WriteLine("[Modules]");
            TestContext.WriteLine("{0}", list);

            // display some data
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_1_2_Get_ResponseParse_2()
        {
            var response = @"[
  ""Module\\DNNSimpleArticle_00.02.01_Install.zip"",
  ""Module\\UsersExportImport_v.01.01.01.zip""
]";

            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<string[]>(response);

            Assert.AreEqual(2, list.Length);
            Assert.IsTrue(list[0].Contains("DNNSimpleArticle"), "DNNSimpleArticle");
            Assert.IsTrue(list[1].Contains("UsersExportImport"), "UsersExportImport");
        }


        [TestMethod]
        public void InstallFolder_1_1_3_Get_ResponseParse_0()
        {
            var response = @"[
]";

            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<string[]>(response);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void InstallFolder_1_1_4_GetFiles()
        {
            var installFolder = @"C:\inetpub\dnn734\Install";
            var packageName = "*";

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(installFolder, "*" + packageName + "*.zip", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(installFolder, "*" + packageName + "*.resources", SearchOption.AllDirectories));

            foreach (var file in files)
            {
                TestContext.WriteLine("File: '{0}'", file.Replace(installFolder + @"\", ""));
            }
        }

        [TestMethod]
        public void InstallFolder_1_2_Count()
        {
            var count = client.Count();
            TestContext.WriteLine("count: {0}", count);

            // display some data
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_3_Save()
        {
            var uploaded = client.Save(SampleData.ModulePathBlog);
            TestContext.WriteLine("saved: {0}", uploaded);

        }

        [TestMethod]
        public void InstallFolder_1_4_InstallResources()
        {
            client.Clear();
            var uploaded = client.Save(SampleData.ModulePathBlogUpgrade);
            client.InstallResources();
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_5_1_Delete()
        {
            client.Delete("Module", "blog");
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_5_2_Delete()
        {
            client.Delete("Module", "simplearticle", "UsersExport");
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_6_Clear()
        {
            client.Clear();
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_2_1_Blog_Downgrade()
        {
            client.Save(SampleData.ModulePathBlog);
            CheckAndDisplayResponse(client);

            client.Save(SampleData.ModulePathBlogDowngrade);
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_2_2_Blog_Upgrade()
        {
            client.Save(SampleData.ModulePathBlog);
            CheckAndDisplayResponse(client);

            client.Save(SampleData.ModulePathBlogUpgrade);
            CheckAndDisplayResponse(client);
        }


        [TestMethod]
        public void InstallFolder_2_3_Save_All()
        {
            client.Save(SampleData.ModulePathBlog, SampleData.ModulePathSimpleArticle, SampleData.ModulePathUserExport);
            CheckAndDisplayResponse(client);
        }
    }
}
