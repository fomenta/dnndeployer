using CommandLine;

namespace dnncmd.Arguments
{
    internal class ModuleOptions : CommonOptions
    {
        [Option('i', "install", DefaultValue = false,
                    HelpText = "Use this flag if you want to install a module.")]
        public bool Install { get; set; }

        [Option('u', "uninstall", DefaultValue = false,
                    HelpText = "Use this flag if you want to uninstall a module.")]
        public bool Uninstall { get; set; }

        [Option('l', "list", DefaultValue = false, HelpText = "Find details for a given module or a set of modules.")]
        public bool List { get; set; }

        [Option('d', "listdesktop", DefaultValue = false, HelpText = "Find details for a given desktop module or a set of modules.")]
        public bool ListDesktop { get; set; }

        [Option('m', "module",
            HelpText = "Path to DotNetNuke module package (install) or module name (uninstall).")]
        public string Module { get; set; }

        [Option('p', "pattern", HelpText = "Filter Pattern (regex) when searching for installed modules (GET).")]
        public string Pattern { get; set; }

        [Option('b', "builtin", HelpText = "Add this flag if you want to display built-in DotNetNuke modules.")]
        public bool BuiltIn { get; set; }

        [Option('f', "force", DefaultValue = false,
                    HelpText = "Delete module if found installed already. Useful when forcing downgrade (when installing a lower version of an existing module). WARNING: Module will be removed from existing pages.")]
        public bool Force { get; set; }

    }
}
