using CommandLine;
using CommandLine.Text;

namespace dnncmd.Arguments
{
    internal class CommonOptions
    {
        [Option('r', "DotNetNukeRootUrl", Required = true,
             HelpText = "Root URL to the DotNetNuke location where the module will be installed to.")]
        public string DotNetNukeRootUrl { get; set; }

        [Option('v', "version", DefaultValue = false,
                    HelpText = "Displays DotNetNuke Deployer version installed on target DotNetNuke.")]
        public bool Version { get; set; }

        [Option('y', "verify", DefaultValue = false,
                    HelpText = "Verify if DotNetNuke Deployer is installed on target DotNetNuke.")]
        public bool Verify { get; set; }

        [Option("verbose", DefaultValue = false,
                    HelpText = "Displays additional information of execution details.")]
        public bool Verbose { get; set; }

        [Option("user", HelpText = "DotNetNuke Authentication User name.")]
        public string UserName { get; set; }

        [Option('p', "password", HelpText = "DotNetNuke Authentication User password.")]
        public string Password { get; set; }

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
