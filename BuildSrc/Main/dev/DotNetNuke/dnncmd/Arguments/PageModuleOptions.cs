
using CommandLine;
namespace dnncmd.Arguments
{
    internal class PageModuleOptions : CommonOptions
    {
        #region Actions
        // WAS: ModulesAdd
        [Option('a', "add", HelpText = "Add a module to a given page. Optionally clear all modules previosly added")]
        public bool Add { get; set; }

        // WAS: ModulesList
        [Option('g', "get", HelpText = "Get details for one or all module(s) within a page.")]
        public bool Get { get; set; }

        // WAS: ModulesClear
        [Option('c', "clear", HelpText = "Clear all modules added to a given page.")]
        public bool Clear { get; set; }
        #endregion

        #region Parameters
        [Option("portalalias", HelpText = "DNN portal alias for the target portal.")]
        public string PortalAlias { get; set; }
        [Option("portalid", HelpText = "DNN internal ID for the target portal.")]
        public int? PortalID { get; set; }

        [Option("id", HelpText = "DNN internal ID for the request page.")]
        public int? TabID { get; set; }
        [Option("path", HelpText = "DNN page path (e.g. '//NoWhitespaceName/Child/Grandchild') for the request page.")]
        public string TabPath { get; set; }
        [Option("fullname", HelpText = "DNN page full name (e.g. 'Parent Name/Child Name/Grandchild') for the request page.")]
        public string TabFullName { get; set; }


        [Option("module", HelpText = "Specifies the module name to be added to the target page")]
        public string ModuleName { get; set; }

        [Option("title", HelpText = "Specifies the module title to be displayed within the target page")]
        public string ModuleTitle { get; set; }

        [Option("pane", HelpText = "Optional. Specifies the pane name on which to add ModuleName to the target page. Default value is ContentPane")]
        public string PaneName { get; set; }

        [Option("keepexisting", HelpText = "Optional. Set to 'true' if want to keep all modules before adding requested module to the target page. Default value is clear all modules before adding a new one.")]
        public bool KeepExisting { get; set; }

        [Option("settingname", HelpText = "Specifies the name for a setting to be associated to the page/module combination")]
        public string SettingName { get; set; }

        [Option("settingvalue", HelpText = "Specifies the value for the corresponding SettingName")]
        public string SettingValue { get; set; }
        #endregion
    }
}
