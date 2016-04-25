using Build.DotNetNuke.Deployer.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Build.Extensions.Tests.DotNetNuke
{
    [TestClass]
    public class InstallFolderAdminClientTests : BaseUnitTest<InstallFolderAdminClient>
    {
        [TestMethod]
        public void InstallFolder_1_1_Get()
        {
            var list = client.Get();
            TestContext.WriteLine("list: {0}", list);

            // display some data
            CheckAndDisplayResponse(client);
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
        public void InstallFolder_1_4_Delete()
        {
            client.Delete("Module", "blog");
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_1_4_Clear()
        {
            client.Clear();
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_2_1_Blog_Downgrade()
        {
            client.Save(SampleData.ModulePathBlog);
            CheckAndDisplayResponse(client);

            client.Delete("Module", "blog");
            CheckAndDisplayResponse(client);

            client.Save(SampleData.ModulePathBlogDowngrade);
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void InstallFolder_2_2_Blog_Upgrade()
        {
            client.Save(SampleData.ModulePathBlog);
            CheckAndDisplayResponse(client);

            client.Delete("Module", "blog");
            CheckAndDisplayResponse(client);

            client.Save(SampleData.ModulePathBlogUpgrade);
            CheckAndDisplayResponse(client);
        }

    }
}
