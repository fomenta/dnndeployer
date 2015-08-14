using Build.Extensions.Tests.Mocking;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.RegularExpressions;
using af = TfsBuildExtensions.Activities.FileSystem;

namespace Build.Extensions.Tests.Activities
{
    [TestClass]
    public class FileActivityTests : BaseUnitTest
    {
        private const string PackageVersionXPath = @"dotnetnuke/packages/package/@version";
        private const string TargetVersionNumber = "11.02.01.03";
        private const string RegexPattern = @"(<package .*version="")" + @"[^""]+";
        private readonly string Replacement = "${1}" + TargetVersionNumber;


        [TestMethod]
        public void TestRegexReplace()
        {
            string sampleDnnDefinitionFile = CopyToTemporaryFile(SampleData.DnnSampleDefinitionPath);
            string input = File.ReadAllText(sampleDnnDefinitionFile);
            var newFileContents = Regex.Replace(input, RegexPattern, Replacement);
            TestContext.WriteLine("[UPDATED CONTENTS]");
            TestContext.WriteLine("{0}", newFileContents);
        }


        [TestMethod]
        public void XamlFileActivityRegex()
        {
            string sampleDnnDefinitionFile = CopyToTemporaryFile(SampleData.DnnSampleDefinitionPath);
            TestContext.WriteLine("Dnn Definition File Path: {0}", sampleDnnDefinitionFile);

            string actualVersion = XmlGetValue(sampleDnnDefinitionFile, PackageVersionXPath);
            Assert.AreNotEqual(TargetVersionNumber, actualVersion, "Original package version must be different from the one to be assigned");

            var xamlInjector = new XamlInjector(SampleData.XamlInvokeFileActity);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            // in args
            host.InArguments.Files = new[] { sampleDnnDefinitionFile };
            host.InArguments.RegexPattern = RegexPattern;
            host.InArguments.Replacement = Replacement;

            try { host.TestActivity(); }
            finally { host.Tracking.Trace(); }

            TestContext.WriteLine("[UPDATED CONTENTS]");
            TestContext.WriteLine("{0}", File.ReadAllText(sampleDnnDefinitionFile));

            string updatedVersion = XmlGetValue(sampleDnnDefinitionFile, PackageVersionXPath);
            Assert.AreEqual(TargetVersionNumber, updatedVersion, "Updated package version");
        }


        [TestMethod]
        public void DirectFileActivityRegex()
        {
            string sampleDnnDefinitionFile = CopyToTemporaryFile(SampleData.DnnSampleDefinitionPath);

            string actualVersion = XmlGetValue(sampleDnnDefinitionFile, PackageVersionXPath);
            Assert.AreNotEqual(TargetVersionNumber, actualVersion, "Original package version must be different from the one to be assigned");

            // Arrange
            var activity = new af.File();
            var host = WorkflowInvokerTest.Create(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            host.InArguments.FailBuildOnError = true;
            host.InArguments.Files = new[] { sampleDnnDefinitionFile };
            host.InArguments.RegexPattern = RegexPattern;
            host.InArguments.Replacement = Replacement;

            try { host.TestActivity(); }
            finally { host.Tracking.Trace(); }

            TestContext.WriteLine("[UPDATED CONTENTS]");
            TestContext.WriteLine("{0}", File.ReadAllText(sampleDnnDefinitionFile));

            string updatedVersion = XmlGetValue(sampleDnnDefinitionFile, PackageVersionXPath);
            Assert.AreEqual(TargetVersionNumber, updatedVersion, "Updated package version");
        }
    }
}
