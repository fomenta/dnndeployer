using CommandLine;
using CommandLine.Text;

namespace Build.DotNetNuke.Deployer.Client.ConsoleArguments
{
    internal class LocalOptions
    {
        [Option("version", DefaultValue = false,
            HelpText = "Displays this exe's version.")]
        public bool Version { get; set; }
        
        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
