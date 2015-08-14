using Build.Extensions.Tests.Mocking;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Build.Extensions.Tests.Activities
{
    [TestClass]
    public class DirectoryActivitiesTests : BaseUnitTest
    {
        [TestMethod]
        public void XamlCreateDirectory()
        {
            var xamlInjector = new XamlInjector(SampleData.XamlInvokeCreateDirectory);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            // in args
            host.InArguments.DirFullPath = @"C:\Temp\Subfolder does not exist";

            TestContext.WriteLine("[XamlCreateDirectory]");
            try { host.TestActivity(TimeSpan.FromSeconds(WORKFLOW_RUN_TIMEOUT_SEC)); }
            finally { host.Tracking.Trace(); }
        }

        [TestMethod]
        public void XamlDeleteDirectory()
        {
            var xamlInjector = new XamlInjector(SampleData.XamlInvokeDeleteDirectory);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            // in args
            host.InArguments.DirFullPath = @"C:\Temp\Subfolder does not exist";

            TestContext.WriteLine("[XamlDeleteDirectory]");
            try { host.TestActivity(TimeSpan.FromSeconds(WORKFLOW_RUN_TIMEOUT_SEC)); }
            finally { host.Tracking.Trace(); }
        }
    }
}
