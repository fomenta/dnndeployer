using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using dnn = DotNetNuke.Entities.Modules;

namespace Build.DotNetNuke.Deployer.Services
{
    [RequireHost]
    public class PageModuleController : AdminBaseController
    {
        // default page with a DNN page to place modules to be added (when not specified)
        public const string PAGE_DEFAULT_PANE = "ContentPane";

        #region Modules on Page
        [HttpPut]
        public HttpResponseMessage Add(PageModulesAddRequest request)
        {
            TabInfo objTab = null;
            dnn.ModuleController moduleCtrl = null;
            HttpResponseMessage response;
            var page = request.Page;

            // if requested, clear existing module before adding new ones
            if (request.ClearExisting)
            {
                response = ModulesClearInternal(out objTab, out moduleCtrl,
                                                page.TabFullName, page.TabPath, page.ID, page.PortalAlias, page.PortalID);
                if (response != null) { return response; }
            }
            else
            {
                response = ModuleResolvePage(out objTab, page.TabFullName, page.TabPath, page.ID, page.PortalAlias, page.PortalID);
                if (response != null) { return response; }
                moduleCtrl = new dnn.ModuleController();
            }

            // check module exists and get module details
            ModuleInfo moduleInfo = null;
            response = ModuleValidateAndAdd(request.Module, ref moduleInfo);
            if (response != null) { return response; }

            // add module to page
            ModuleAddToPage(moduleInfo, objTab, moduleCtrl);

            // [2015-03-27] allow adding page-module settings, especially used with Code OnTime with parameter DefaultUserControl
            if (!string.IsNullOrWhiteSpace(request.Module.SettingName))
            { ModuleAddSettings(moduleCtrl, moduleInfo.ModuleID, request.Module.SettingName, request.Module.SettingValue); }

            return Request.CreateResponse(HttpStatusCode.OK, moduleInfo.ModuleID, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage Get(string tabFullName = null, string tabPath = null, int? id = null,
                                       string portalAlias = null, int? portalID = null)
        {
            TabInfo objTab = null;
            var response = ModuleResolvePage(out objTab, tabFullName, tabPath, id, portalAlias, portalID);
            if (response != null) { return response; }

            var moduleCtrl = new dnn.ModuleController();
            //get all tabModule Instances
            var list = new List<dynamic>();
            foreach (ModuleInfo m in moduleCtrl.GetTabModules(objTab.TabID).Values)
            {
                var dm = m.DesktopModule;
                var mc = m.ModuleControl;
                var md = m.ModuleDefinition;

                list.Add(new
                {
                    KeyID = m.KeyID,
                    PaneName = m.PaneName,
                    ModuleControlId = m.ModuleControlId,
                    ModuleTitle = m.ModuleTitle,
                    Settings = m.ModuleSettings,
                    //
                    DesktopModule_DesktopModuleID = dm.DesktopModuleID,
                    DesktopModule_ModuleName = dm.ModuleName,
                    DesktopModule_FriendlyName = dm.FriendlyName,
                    DesktopModule_Description = dm.Description,
                    DesktopModule_FolderName = dm.FolderName,
                    DesktopModule_CodeSubDirectory = dm.CodeSubDirectory,
                    DesktopModule_Category = dm.Category,
                    DesktopModule_PackageID = dm.PackageID,
                    DesktopModule_ModuleID = dm.ModuleID,
                    DesktopModule_ModuleDefinitions_Count = dm.ModuleDefinitions.Count,
                    DesktopModule_Version = dm.Version,
                    //
                    ModuleControl_ModuleControlID = mc.ModuleControlID,
                    ModuleControl_ControlKey = mc.ControlKey,
                    ModuleControl_ControlSrc = mc.ControlSrc,
                    //
                    ModuleDefinition_ModuleDefID = md.ModuleDefID,
                    ModuleDefinition_DefinitionName = md.DefinitionName,
                    /*
                    // 'DotNetNuke.Entities.Modules.ModuleInfo.AuthorizedEditRoles' is obsolete: 'Deprecated in DNN 5.1. All permission checks are done through Permission Collections'
                    AuthorizedEditRoles = m.AuthorizedEditRoles,
                    // 'DotNetNuke.Entities.Modules.ModuleInfo.AuthorizedViewRoles' is obsolete: 'Deprecated in DNN 5.1. All permission checks are done through Permission Collections'
                    AuthorizedViewRoles = m.AuthorizedViewRoles,
                    */
                    PortalID = m.PortalID,
                    TabModuleSettings_Count = m.TabModuleSettings.Count,
                    TabModuleID = m.TabModuleID,
                    ParentTab_TabID = m.ParentTab.TabID,
                    ParentTab_TabName = m.ParentTab.TabName,
                    ParentTab_TabOrder = m.ParentTab.TabOrder,
                    ParentTab_TabPath = m.ParentTab.TabPath,

                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, list, new MediaTypeHeaderValue("text/json"));
        }

        [HttpDelete]
        public HttpResponseMessage Clear(PageGetRequest page)
        {
            var response = ModulesClearInternal(page.TabFullName, page.TabPath, page.ID, page.PortalAlias, page.PortalID);
            if (response != null) { return response; }

            return Request.CreateResponse(HttpStatusCode.OK, true, new MediaTypeHeaderValue("text/json"));
        }
        #endregion

        #region Private
        private void ModuleAddSettings(dnn.ModuleController moduleCtrl, int ModuleID, string settingName, string settingValue)
        {
            moduleCtrl.UpdateModuleSetting(ModuleID, settingName, settingValue);
        }

        private int GetPortalIDByAlias(string portalAlias)
        {
            var portalAliasInfo = TestablePortalAliasController.Instance.GetPortalAlias(portalAlias);
            // it could be that the site alias was missing (e.g. specified 'alberta' and the full alias is 'dnndev.me/alberta')
            if (portalAliasInfo == null)
            {
                // e.g. dnndev.me
                string rootUrlHostOnly = HttpContext.Current.Request.Url.Host;
                // e.g. dnndev.me/alberta
                portalAlias = string.Format("{0}/{1}", rootUrlHostOnly, portalAlias);
                // give it another try with the new alias    
                portalAliasInfo = TestablePortalAliasController.Instance.GetPortalAlias(portalAlias);
            }

            if (portalAliasInfo == null)
            { throw new ArgumentOutOfRangeException("portalAlias", string.Format(Resources.PortalAliasNotFound, portalAlias)); }

            // get back id
            return portalAliasInfo.PortalID;
        }

        private int ResolvePortalId(int? portalID, string portalAlias, int? defaultUserID = null)
        {
            int myPortalID;
            if (!portalID.HasValue && string.IsNullOrWhiteSpace(portalAlias))
            {
                if (defaultUserID.HasValue) { return defaultUserID.Value; }
                else { throw new ArgumentNullException("portalID", "Either portalID or portalAlias must be specified."); }
            }
            else if (!string.IsNullOrEmpty(portalAlias))
            { myPortalID = GetPortalIDByAlias(portalAlias); }
            else
            { myPortalID = portalID.Value; }

            return myPortalID;
        }

        private int ResolveTabId(int? tabID, string tabPath, string tabFullName, int portalID)
        {
            if (tabID.HasValue) { return tabID.Value; }
            if (string.IsNullOrWhiteSpace(tabFullName))
            { return TabController.GetTabByTabPath(portalID, tabPath, Null.NullString); }
            else
            {
                var tabs = new Hashtable(PageListToDictionary(portalID: portalID));
                return tabs[tabFullName] == null ? Null.NullInteger : (int)tabs[tabFullName];
            }
        }

        private Dictionary<string, int> PageListToDictionary(string portalAlias = "", int? portalID = null)
        {
            int myPortalID = ResolvePortalId(portalID, portalAlias, UserInfo.PortalID);


            List<TabInfo> listTabs = TabController.GetPortalTabs(myPortalID, Null.NullInteger,
                                                    includeNoneSpecified: false, noneSpecifiedText: null,
                                                    includeHidden: true, includeDeleted: true, includeURL: false,
                                                    checkViewPermisison: false, checkEditPermission: true);

            var tabs = (from tab in listTabs
                        select new { Key = GetPageBreadcrumb(tab, listTabs), tab.TabID }
                       ).ToDictionary(t => t.Key, t => t.TabID);

            return tabs;
        }

        private string GetPageBreadcrumb(TabInfo tab, List<TabInfo> listTabs)
        {
            if (tab == null) { return ""; }
            var breadcrumb = GetPageBreadcrumb(listTabs.Where(x => x.TabID == tab.ParentId).FirstOrDefault(), listTabs);

            return string.IsNullOrWhiteSpace(breadcrumb)
                        ? string.Format("{0}", tab.TabName)
                        : string.Format("{0}/{1}", breadcrumb, tab.TabName);

        }


        private HttpResponseMessage ModuleResolvePage(out TabInfo objTab, string tabFullName = null, string tabPath = null, int? id = null,
                                              string portalAlias = null, int? portalID = null)
        {
            int myPortalID = ResolvePortalId(portalID, portalAlias, UserInfo.PortalID);
            int myTabID = ResolveTabId(id, tabPath, tabFullName, myPortalID);

            objTab = new TabController().GetTab(myTabID, myPortalID, ignoreCache: false);

            // page does not exist, return false (but do not generate error)
            if (myTabID == Null.NullInteger)
            { return Request.CreateResponse(HttpStatusCode.OK, false, new MediaTypeHeaderValue("text/json")); }

            return null;
        }


        private HttpResponseMessage ModuleValidateAndAdd(PageModuleDto item, ref ModuleInfo module)
        {
            return ModuleValidateAndAdd(item, ref module, null);
        }

        private HttpResponseMessage ModuleValidateAndAdd(PageModuleDto item, ref ModuleInfo module, List<ModuleInfo> modules)
        {
            // make sure module to be added to page is already installed
            DesktopModuleInfo desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(item.ModuleName, UserInfo.PortalID);
            if (desktopModule == null || desktopModule.DesktopModuleID == Null.NullInteger)
            { return ResponseNotFound(Resources.ModuleDefinitionNotFoundForName, item.ModuleName); }

            ModuleDefinitionInfo moduleDefinition = (from d in ModuleDefinitionController.GetModuleDefinitions()
                                                     where d.Value.DesktopModuleID == desktopModule.DesktopModuleID
                                                     select d.Value).FirstOrDefault();
            if (moduleDefinition == null || moduleDefinition.ModuleDefID == Null.NullInteger)
            { return ResponseNotFound(Resources.ModuleDefinitionNotFoundForName, item.ModuleName); }

            module = new ModuleInfo
            {
                ModuleDefID = moduleDefinition.ModuleDefID,
                //DesktopModuleID = desktopModule.DesktopModuleID,
                //ModuleID = desktopModule.ModuleID,
                //
                ModuleTitle = item.ModuleTitle ?? item.ModuleName,
                PaneName = item.PaneName ?? PAGE_DEFAULT_PANE,
                //
                InheritViewPermissions = true,
            };
            // add module
            if (modules != null) { modules.Add(module); }
            return null;
        }

        private void ModuleAddToPage(ModuleInfo module, TabInfo objTab, dnn.ModuleController moduleCtrl)
        {
            module.TabID = objTab.TabID;
            module.PortalID = objTab.PortalID;
            module.CultureCode = objTab.CultureCode;
            // add module
            module.ModuleID = moduleCtrl.AddModule(module);
        }

        private HttpResponseMessage ModulesClearInternal(string tabFullName = null, string tabPath = null, int? id = null,
                                                         string portalAlias = null, int? portalID = null)
        {
            TabInfo objTab = null;
            dnn.ModuleController moduleCtrl = null;
            return ModulesClearInternal(out objTab, out moduleCtrl,
                                        tabFullName, tabPath, id, portalAlias, portalID);
        }

        private HttpResponseMessage ModulesClearInternal(out TabInfo objTab, out dnn.ModuleController moduleCtrl,
                                                         string tabFullName = null, string tabPath = null, int? id = null,
                                                         string portalAlias = null, int? portalID = null)
        {
            objTab = null;
            moduleCtrl = new dnn.ModuleController();
            var response = ModuleResolvePage(out objTab, tabFullName, tabPath, id, portalAlias, portalID);
            if (response != null) { return response; }

            //Delete all tabModule Instances
            foreach (ModuleInfo m in moduleCtrl.GetTabModules(objTab.TabID).Values)
            { moduleCtrl.DeleteTabModule(m.TabID, m.ModuleID, false); }

            return null;
        }
        #endregion
    }
}