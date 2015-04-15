using Microsoft.TeamFoundation.Build.Client;
using System.Activities;
using System.IO;
using TfsBuildExtensions.Activities;

namespace Build.Extensions.Activities
{
    [BuildActivity(HostEnvironmentOption.All)]
    public class FileAttribActivity : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<string> SearchPathAndPattern { get; set; }

        public InArgument<bool> IsReadOnly { get; set; }

        protected override void InternalExecute()
        {
            var searchPathAndPattern = SearchPathAndPattern.Get(this.ActivityContext);
            var isReadOnly = IsReadOnly.Get(this.ActivityContext);

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

            this.LogBuildMessage(string.Format("Making files {0}", isReadOnly ? "Read-Only" : "Writeable"));
            this.LogBuildMessage(string.Format("path: '{0}'", path));
            this.LogBuildMessage(string.Format("searchPattern: '{0}'", searchPattern));

            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            { this.LogBuildError(string.Format("Directory does not exist: '{0}'", path)); return; }

            var files = dirInfo.GetFiles(searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                file.IsReadOnly = isReadOnly;
                this.LogBuildMessage(file.FullName, BuildMessageImportance.High);
            }
        }


    }
}
