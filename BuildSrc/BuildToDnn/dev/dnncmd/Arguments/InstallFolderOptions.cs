using CommandLine;

namespace Build.DotNetNuke.Deployer.Client.ConsoleArguments
{
    internal class InstallFolderOptions : CommonOptions
    {
        [Option('c', "count", DefaultValue = false,
                    HelpText = "Return a count of files pending to install.")]
        public bool Count { get; set; }

        [Option('g', "get", DefaultValue = false,
                    HelpText = "Get a list of files pending to be installed.")]
        public bool Get { get; set; }

        [Option('s', "save", DefaultValue = false, HelpText = "Save modules to install folder.")]
        public bool Save { get; set; }

        [Option('d', "delete", DefaultValue = false, HelpText = "Delete modules from the install folder.")]
        public bool Delete { get; set; }

        [Option("clear", DefaultValue = false, HelpText = "Clear all modules from the install folder.")]
        public bool Clear { get; set; }

        [Option('p', "packageType", DefaultValue = false, HelpText = "Specifies a specific package type of blank for all.")]
        public string PackageType { get; set; }

        [OptionArray('m', "modules", HelpText = "Path to modules package to be install.")]
        public string[] Modules { get; set; }
    }
}
