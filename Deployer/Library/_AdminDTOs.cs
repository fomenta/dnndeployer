using System.Collections.Generic;
using System.Linq;

namespace Build.DotNetNuke.Deployer.Library
{
    public class DeployResponse
    {
        public bool Success { get; set; }
        public int AffectedItems { get; set; }
        public string ErrorMessage { get; set; }
        public List<DeployResponseItem> Files { get; set; }
    }

    public class DeployResponseItem
    {
        public string Extension { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public bool NotFound { get; set; }
    }

    public class PortalDto
    {
        public int PortalID { get; set; }
        public string PortalName { get; set; }
        public string PortalFirstAlias { get; set; }
        public string PortalFirstUrl { get; set; }
    }

    public class PageGetRequest
    {
        #region Constructors
        public PageGetRequest() { }

        public PageGetRequest(string tabPath = null, string tabFullName = null, int? id = null,
                               string portalAlias = null, int? portalID = null)
        {
            TabPath = tabPath;
            TabFullName = tabFullName;
            ID = id;
            PortalAlias = portalAlias;
            PortalID = portalID;
        }
        #endregion

        public string TabPath { get; set; }
        public string TabFullName { get; set; }
        public int? ID { get; set; }
        public string PortalAlias { get; set; }
        public int? PortalID { get; set; }
    }

    public class PageGetResponse
    {
        public int TabID { get; set; }
        public string TabName { get; set; }
        public string TabPath { get; set; }
        public int ParentId { get; set; }
        public int? PortalID { get; set; }
    }

    public class PageListItemDto : PageGetResponse
    {
        public string Title { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisible { get; set; }
        public bool IsSuperTab { get; set; }
        public int Level { get; set; }
        public string ParentPath { get; set; }
    }

    public class PageInfoDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string IconFile { get; set; }
        public string IconFileLarge { get; set; }
        public string PortalAlias { get; set; }
        public int? PortalID { get; set; }

        // full page path in the form of: "//NonWhitespacePath//Child//Grandchild"
        public string ParentPagePath { get; set; }
        public string BeforePagePath { get; set; }
        public string AfterPagePath { get; set; }

        public List<PagePermissionDto> Permissions { get; set; }
        public List<PageModuleDto> Modules { get; set; }

        // full page name in the form of: "PageName/Child Name/Grandchild Name"
        public string ParentPageFullName { get; set; }
        public string BeforePageFullName { get; set; }
        public string AfterPageFullName { get; set; }
    }

    public class PageModuleDto
    {
        public const string DEFAULT_PANE = "ContentPane";

        public string ModuleName { get; set; }
        public string ModuleTitle { get; set; }
        public string PaneName { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        public PageModuleDto()
        {
            this.PaneName = DEFAULT_PANE;
        }
        public PageModuleDto(string moduleName, string moduleTitle = null, string paneName = null, string settingName = null, string settingValue = null)
        {
            this.ModuleName = moduleName;
            this.ModuleTitle = moduleTitle ?? moduleName;
            if (!string.IsNullOrWhiteSpace(paneName)) { this.PaneName = paneName; }
            this.SettingName = settingName;
            this.SettingValue = settingValue;
        }


        /// <summary>
        /// Modules to be added to the target page.
        /// Example: 
        ///     FriendlyName[,ContentPane];...
        ///     FriendlyName,[ContentPane,]ModuleTitle;...
        /// </summary>
        /// <returns></returns>
        public static List<PageModuleDto> ToList(string moduleString)
        {
            if (string.IsNullOrWhiteSpace(moduleString)) { return null; }
            var modules = new List<PageModuleDto>();

            var moduleKeyPairs = moduleString.Split(';');
            foreach (var moduleKeyPair in moduleKeyPairs)
            {
                var moduleParts = moduleKeyPair.Split(',');
                var moduleName = moduleParts[0];
                var pageModule = new PageModuleDto();

                pageModule.ModuleName = moduleParts[0];
                pageModule.PaneName = moduleParts.Length < 2 ? null : moduleParts[1];
                pageModule.ModuleTitle = moduleParts.Length < 3 ? pageModule.ModuleName : string.Join(",", moduleParts.Skip(2));

                modules.Add(pageModule);
            }

            return modules;
        }
    }

    public class PagePermissionDto
    {
        public const string KEY_VIEW = "VIEW";
        public const string KEY_EDIT = "EDIT";

        public string PermissionKey { get; set; }
        public string RoleName { get; set; }

        public PagePermissionDto() { }
        public PagePermissionDto(string RoleName)
        {
            this.RoleName = RoleName;
            this.PermissionKey = KEY_VIEW;
        }

        public PagePermissionDto(string RoleName, string PermissionKey)
        {
            this.RoleName = RoleName;
            this.PermissionKey = PermissionKey;
        }

        /// <summary>
        /// Permissions to be added to the target page (Administrators by default are added). 
        /// Syntax: 
        ///     {RoleName[,VIEW];RoleName,VIEW|EDIT;...}
        /// </summary>
        public static List<PagePermissionDto> ToList(string permissionString)
        {
            if (string.IsNullOrWhiteSpace(permissionString)) { return null; }
            var permissions = new List<PagePermissionDto>();

            var permissionKeyPairs = permissionString.Split(';');
            foreach (var permissionKeyPair in permissionKeyPairs)
            {
                var roleNameAndPermissionKeys = permissionKeyPair.Split(',');
                var roleName = roleNameAndPermissionKeys[0];

                if (roleNameAndPermissionKeys.Length == 1)
                { permissions.Add(new PagePermissionDto(roleName)); }
                else
                {
                    var permissionKeys = roleNameAndPermissionKeys[1].Split('|');
                    foreach (var item in permissionKeys)
                    { permissions.Add(new PagePermissionDto(roleName, item)); }
                }
            }

            return permissions;
        }
    }

    public class PageModulesAddRequest
    {
        /// <summary>
        /// reference to the module to be added
        /// </summary>
        public PageModuleDto Module { get; set; }

        /// <summary>
        /// reference to the page on which to add the module
        /// </summary>
        public PageGetRequest Page { get; set; }

        /// <summary>
        /// Clear existing module before adding the new one on this request
        /// </summary>
        public bool ClearExisting { get; set; }
    }
}
