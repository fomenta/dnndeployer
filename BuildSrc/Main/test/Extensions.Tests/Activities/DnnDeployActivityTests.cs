using Build.Extensions.Tests.Mocking;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Build.Extensions.Tests.Activities
{
    [TestClass]
    public class DnnDeployActivityTests : BaseUnitTest
    {
        [TestMethod]
        public void XamlDnnDeploy()
        {
            var modulePath = SampleData.ModulePathBlog;
            var moduleName = SampleData.ModuleNameBlog;
            ModuleUninstall(moduleName);

            var xamlInjector = new XamlInjector(SampleData.XamlInvokeDnnDeploy);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            // in args
            host.InArguments.TargetDnnRootUrl = DNN_URL;
            host.InArguments.ModuleFilePath = SampleData.ModulePathBlog;

            TestContext.WriteLine("[XamlInvokeDnnDeploy]");
            try { host.TestActivity(TimeSpan.FromSeconds(WORKFLOW_RUN_TIMEOUT_SEC)); }
            finally { host.Tracking.Trace(); }
        }
    }
}
