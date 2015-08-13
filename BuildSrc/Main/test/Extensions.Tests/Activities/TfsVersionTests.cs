using Build.Extensions.Tests.Mocking;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Build.Extensions.Tests.Activities
{
    [TestClass]
    public class TfsVersionTests : BaseUnitTest
    {
        [TestMethod]
        public void SetVersionTest()
        {
            var xamlInjector = new XamlInjector(SampleData.XamlInvokeTfsVersion);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            var assemblyVersion = "1.1.0.0";
            var fileVersion = "1.1.*.*";

            // in args
            host.InArguments.VersioningAssemblyVersion = assemblyVersion;
            host.InArguments.VersioningFileVersion = fileVersion;
            host.InArguments.VersioningAssemblyDescription = "Description";

            host.InArguments.MatchingFiles =
                new[] { SampleData.SampleAssemblyInfo1, SampleData.SampleAssemblyInfo2 };
            host.InArguments.CSProj =
                new[] { SampleData.SampleCsProj1, SampleData.SampleCsProj2 };
            host.InArguments.DnnModuleDefinitionFiles =
                new[] { SampleData.SampleDnnPackageManifest1, SampleData.SampleDnnPackageManifest2 };
            host.InArguments.SourcesDirectory = SampleData.AssemblyInfoSamples;
            host.InArguments.CodeBuildNumber = 679;

            try { host.TestActivity(); }
            finally { host.Tracking.Trace(); }
        }

        [TestMethod]
        public void ChangeInArgumentTest()
        {
            var xamlInjector = new XamlInjector(SampleData.XamlChangeInArgument);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            var assemblyVersion = "1.1.0.0";
            var fileVersion = "1.0.*.*";

            // in args
            host.InArguments.VersioningAssemblyVersion = assemblyVersion;
            host.InArguments.VersioningFileVersion = fileVersion;
            host.InArguments.MatchingFiles =
                new[] { SampleData.SampleAssemblyInfo1, SampleData.SampleAssemblyInfo2 };
            host.InArguments.CodeBuildNumber = 2412;

            try { host.TestActivity(); }
            finally { host.Tracking.Trace(); }
        }
    }
}
