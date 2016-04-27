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

        [Option('s', "save", DefaultValue = false, HelpText = "Save dnn extensions to install folder.")]
        public bool Save { get; set; }

        [Option('i', "installresources", DefaultValue = false, HelpText = "Install all dnn extensions found under install folder.")]
        public bool InstallResources { get; set; }

        [Option('d', "delete", DefaultValue = false, HelpText = "Delete dnn extension from the install folder.")]
        public bool Delete { get; set; }

        [Option("clear", DefaultValue = false, HelpText = "Clear all dnn extensions from the install folder.")]
        public bool Clear { get; set; }

        [Option("packagetype", HelpText = "Specifies a specific dnn extension type of blank for all.")]
        public string PackageType { get; set; }

        [OptionArray('m', "modules", HelpText = "Path to dnn extensions package to be install.")]
        public string[] Modules { get; set; }
    }
}
