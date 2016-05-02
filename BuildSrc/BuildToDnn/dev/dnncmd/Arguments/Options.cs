using CommandLine;
using CommandLine.Text;

namespace Build.DotNetNuke.Deployer.Client.ConsoleArguments
{
    // Define a class to receive parsed values
    internal class Options
    {
        [VerbOption("local", HelpText = "Options against local exe (e.g. Check exe version).")]
        public LocalOptions LocalVerb { get; set; }

        [VerbOption("module", HelpText = "Administer modules.")]
        public ModuleOptions ModuleVerb { get; set; }

        [VerbOption("page", HelpText = "Administer pages.")]
        public PageOptions PageVerb { get; set; }

        [VerbOption("pagemodule", HelpText = "Administer modules within a page.")]
        public PageModuleOptions PageModuleVerb { get; set; }

        [VerbOption("installfolder", HelpText = "Administer install folder directory on the web server.")]
        public InstallFolderOptions InstallFolderVerb { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}
