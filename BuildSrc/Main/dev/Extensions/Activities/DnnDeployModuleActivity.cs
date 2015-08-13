using Build.Extensions.DotNetNuke;
using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TfsBuildExtensions.Activities;

namespace Build.Extensions.Activities
{
    [BuildActivity(HostEnvironmentOption.All)]
    public class DnnDeployModuleActivity : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<string> TargetDnnRootUrl { get; set; }

        [Obsolete("Use ModuleFilePaths instead.")]
        public InArgument<string> ModuleFilePath { get; set; }

        public InArgument<IEnumerable<string>> ModuleFilePaths { get; set; }

        public InArgument<string> TargetDnnUserName { get; set; }

        public InArgument<string> TargetDnnPassword { get; set; }

        public InArgument<bool> DeleteModuleFirstIfFound { get; set; }

        protected override void InternalExecute()
        {
            var targetDnnRootUrl = TargetDnnRootUrl.Get(this.ActivityContext);
            var tempFilePath = ModuleFilePath.Get(this.ActivityContext);
            var moduleFilePaths = ModuleFilePaths.Get(this.ActivityContext);

            var userName = TargetDnnUserName.Get(this.ActivityContext);
            var password = TargetDnnPassword.Get(this.ActivityContext);
            var deleteModuleFirstIfFound = DeleteModuleFirstIfFound.Get(this.ActivityContext);

            if (string.IsNullOrWhiteSpace(targetDnnRootUrl)) { this.LogBuildError("TargetDnnRootUrl is required."); return; }
            if (string.IsNullOrWhiteSpace(tempFilePath) && (moduleFilePaths == null || moduleFilePaths.Count() == 0))
            {
                this.LogBuildError("Either ModuleFilePath or ModuleFilePaths are required."); return;
            }

            if (!string.IsNullOrWhiteSpace(tempFilePath)) { moduleFilePaths = new[] { tempFilePath }; }

            var client = new ModuleAdminClient(targetDnnRootUrl, userName, password);
            if (!client.IsDeployerInstalled())
            {
                this.LogBuildError(string.Format(CultureInfo.CurrentCulture, "Deployer Module not installed on '{0}'\nError Message: '{1}'", targetDnnRootUrl, client.LastResponse.ErrorMessage));
                return;
            }

            tempFilePath = string.Join(", ", moduleFilePaths.Select(item => string.Format("\"{0}\"", Path.GetFileName(item))));

            this.LogBuildWarning(string.Format("Installing [{0}] to {1}", tempFilePath, targetDnnRootUrl));
            var success = client.ModuleInstall(deleteModuleFirstIfFound, moduleFilePaths.ToArray());
            if (success)
            { this.LogBuildWarning("Package(s) installed successfully"); }
            else
            { this.LogBuildError(string.Format("Error installing package(s) on '{0}'. ERROR: '{1}'", targetDnnRootUrl, client.LastResponse.Content)); }
        }

    }
}
