
using CommandLine;
namespace dnncmd.Arguments
{
    internal class PageOptions : CommonOptions
    {
        #region Actions
        [Option("portals", HelpText = "Return a list of available portals.")]
        public bool ListPortals { get; set; }

        [Option("portal", HelpText = "Get details for a given portal.")]
        public bool GetPortal { get; set; }

        [Option("portalid", HelpText = "Find out DNN portal id given its alias.")]
        public bool FindOutPortalID { get; set; }

        [Option('a', "add", HelpText = "Add the requested page to DNN.")]
        public bool Add { get; set; }

        [Option('g', "get", HelpText = "Find details for a given page.")]
        public bool Get { get; set; }

        [Option('d', "delete", HelpText = "Delete the requested page.")]
        public bool Delete { get; set; }

        [Option('l', "list", HelpText = "Return a list of pages for a given portal.")]
        public bool List { get; set; }

        [Option('t', "listtodictionary", HelpText = "Return a Dictionary with a list of pages for a given portal.")]
        public bool ListToDictionary { get; set; }

        [Option('i', "import", HelpText = "Import pages from the given page template file.")]
        public bool Import { get; set; }

        // page module actions
        [Option("moduleslist", HelpText = "List all modules added to a given page.")]
        public bool ModulesList { get; set; }

        [Option('m', "modulesadd", HelpText = "Add a module to a given page. Optionally clear all modules previosly added")]
        public bool ModulesAdd { get; set; }

        [Option('c', "modulesclear", HelpText = "Clear all modules added to a given page.")]
        public bool ModulesClear { get; set; }
        #endregion

        #region Parameters
        [Option("file", HelpText = "Full path to the template file to be imported.")]
        public string PageTemplateFilePath { get; set; }

        [Option("portalalias", HelpText = "DNN portal alias for the target portal.")]
        public string PortalAlias { get; set; }
        [Option("portalid", HelpText = "DNN internal ID for the target portal.")]
        public int? PortalID { get; set; }

        [Option("path", HelpText = "DNN page path (e.g. '//NoWhitespaceName/Child/Grandchild') for the request page.")]
        public string TabPath { get; set; }
        [Option("fullname", HelpText = "DNN page full name (e.g. 'Parent Name/Child Name/Grandchild') for the request page.")]
        public string TabFullName { get; set; }

        [Option("id", HelpText = "DNN internal ID for the request page.")]
        public int? TabID { get; set; }

        [Option("recursive", DefaultValue = false, HelpText = "Set to 'true' to apply action to page and its children (e.g. when deleting).")]
        public bool deleteDescendants { get; set; }

        [Option("parentpath", HelpText = "DNN full path (e.g. '//NoWhitespaceName/Child/Grandchild') to locate parent page.")]
        public string ParentPagePath { get; set; }

        [Option("beforepath", HelpText = "DNN full path (e.g. '//NoWhitespaceName/Child/Grandchild') to locate page that should be BEFORE the page to be imported.")]
        public string BeforePagePath { get; set; }

        [Option("afterpath", HelpText = "DNN full path (e.g. '//NoWhitespaceName/Child/Grandchild') to locate page that should be AFTER the page to be imported.")]
        public string AfterPagePath { get; set; }

        [Option("parent", HelpText = "DNN full name (e.g. 'Parent Name/Child Name/Grandchild') to locate parent page.")]
        public string ParentPageFullName { get; set; }

        [Option("before", HelpText = "DNN full name (e.g. 'Parent Name/Child Name/Grandchild') to locate page that should be BEFORE the page to be imported.")]
        public string BeforePageFullName { get; set; }

        [Option("after", HelpText = "DNN full name (e.g. 'Parent Name/Child Name/Grandchild') to locate page that should be AFTER the page to be imported.")]
        public string AfterPageFullName { get; set; }

        [Option("name", HelpText = "Specifies the target page name.")]
        public string Name { get; set; }

        [Option("title", HelpText = "Optional. Specifies the target page title.")]
        public string Title { get; set; }

        [Option("description", HelpText = "Optional. Specifies the target page description.")]
        public string Description { get; set; }

        [Option("url", HelpText = "Optional. Specifies the target page url.")]
        public string Url { get; set; }

        [Option("iconfile", HelpText = "Optional. Specifies the target page icon file.")]
        public string IconFile { get; set; }

        [Option("iconfilelarge", HelpText = "Optional. Specifies the target page large-icon file.")]
        public string IconFileLarge { get; set; }

        [Option("modules", HelpText = "Optional. Modules to be added to the target page (Example: '{<FriendlyName>[,ContentPane];...}' ).")]
        public string Modules { get; set; }

        [Option("permissions", HelpText = "Optional. Permissions to be added to the target page (Administrators by default are added). Syntax: '{<RoleName>[,VIEW];<RoleName>,VIEW|EDIT;...}'.")]
        public string Permissions { get; set; }

        // 
        [Option("module", HelpText = "Specifies the module name to be added to the target page")]
        public string ModuleName { get; set; }

        // 
        [Option("moduletitle", HelpText = "Specifies the module title to be displayed within the target page")]
        public string ModuleTitle { get; set; }

        [Option("pane", HelpText = "Optional. Specifies the pane name on which to add ModuleName to the target page. Default value is ContentPane")]
        public string PaneName { get; set; }

        [Option("keepexisting", HelpText = "Optional. Set to 'true' if want to keep all modules before adding requested module to the target page. Default value is clear all modules before adding a new one.")]
        public bool KeepExisting { get; set; }

        #endregion
    }
}
