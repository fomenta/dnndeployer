using CommandLine;
using CommandLine.Text;

namespace Build.DotNetNuke.Deployer.Client.ConsoleArguments
{
    // Define a class to receive parsed values
    internal class Options
    {
        [VerbOption("module", HelpText = "To administer modules.")]
        public ModuleOptions ModuleVerb { get; set; }

        [VerbOption("page", HelpText = "To administer pages.")]
        public PageOptions PageVerb { get; set; }

        [VerbOption("pagemodule", HelpText = "To administer modules within a page.")]
        public PageModuleOptions PageModuleVerb { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}
