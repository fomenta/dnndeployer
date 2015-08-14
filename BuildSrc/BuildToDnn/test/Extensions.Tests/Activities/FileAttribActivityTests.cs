using Build.Extensions.Tests.Mocking;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Build.Extensions.Tests.Activities
{
    [TestClass]
    public class FileAttribActivityTests : BaseUnitTest
    {
        [TestMethod]
        public void XamlFileAttrib()
        {
            var xamlInjector = new XamlInjector(SampleData.XamlInvokeFileAttrib);
            var activity = xamlInjector.GetActivity();

            var host = new WorkflowInvokerTest(activity);
            host.Invoker.Extensions.Add(new MockBuildDetail());

            var searchPathAndPattern = @"C:\TFS\Facture\PuertoBahia_(TOS)\branches\v01\bin\*";

            // in args
            host.InArguments.SearchPathAndPattern = searchPathAndPattern;
            host.InArguments.IsReadOnly = true;

            try { host.TestActivity(); }
            finally { host.Tracking.Trace(); }
        }

        [TestMethod]
        public void FileAttribTestPatterns()
        {
            //var searchPathAndPattern = @"C:\TFS\Facture\PuertoBahia_(TOS)\branches\v01\bin\*.pdb";
            var searchPathAndPattern = @"C:\TFS\Facture\PuertoBahia_(TOS)\branches\v01\bin\*.*";
            var isReadOnly = true;

            string path, searchPattern;
            if (Directory.Exists(searchPathAndPattern))
            {
                path = searchPathAndPattern;
                searchPattern = "*";
            }
            else
            {
                path = Path.GetDirectoryName(searchPathAndPattern);
                searchPattern = Path.GetFileName(searchPathAndPattern);
            }

            TestContext.WriteLine("path: '{0}'", path);
            TestContext.WriteLine("searchPattern: '{0}'", searchPattern);


            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            { throw new ArgumentException(string.Format("Directory does not exist: '{0}'", path)); }

            var files = dirInfo.GetFiles(searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                file.IsReadOnly = isReadOnly;
                TestContext.WriteLine("[{1}R] {0}", file.FullName, file.IsReadOnly ? "+" : "-");
            }
        }
    }
}
