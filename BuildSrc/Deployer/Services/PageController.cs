using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Xml;
using dnn = DotNetNuke.Entities.Modules;

namespace Build.DotNetNuke.Deployer.Services
{
    [RequireHost]
    public class PageController : AdminBaseController
    {
        // default page with a DNN page to place modules to be added (when not specified)
        public const string PAGE_DEFAULT_PANE = "ContentPane";

        protected override List<string> AllowedExtensionsLowerCase { get { return new List<string> { ".template" }; } }

        protected enum PositionEnum
        {
            AtEnd,
            Before,
            After
        };


        #region Get Portal Information
        [HttpGet]
        public HttpResponseMessage Portals()
        {
            var listPortals = new PortalController().GetPortals();
            var originalUrl = HttpContext.Current.Items["UrlRewrite:OriginalUrl"].ToString().ToLowerInvariant();

            var portals = (from PortalInfo portal in listPortals
                           let alias = TestablePortalAliasController.Instance.GetPortalAliasesByPortalId(portal.PortalID).First()
                           select new PortalDto
                           {
                               PortalID = portal.PortalID,
                               PortalName = portal.PortalName,
                               PortalFirstAlias = alias.HTTPAlias,
                               PortalFirstUrl = Globals.AddPort(Globals.AddHTTP(alias.HTTPAlias), originalUrl),
                           });

            return Request.CreateResponse(HttpStatusCode.OK, portals, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage Portal(string alias = "", int? id = null)
        {
            var myPortalID = ResolvePortalId(id, alias);

            var portalInfo = new PortalController().GetPortal(myPortalID);
            var originalUrl = HttpContext.Current.Items["UrlRewrite:OriginalUrl"].ToString().ToLowerInvariant();
            var httpAlias = TestablePortalAliasController.Instance.GetPortalAliasesByPortalId(myPortalID).First().HTTPAlias;

            var portal = new PortalDto
                         {
                             PortalID = portalInfo.PortalID,
                             PortalName = portalInfo.PortalName,
                             PortalFirstAlias = httpAlias,
                             PortalFirstUrl = Globals.AddPort(Globals.AddHTTP(httpAlias), originalUrl),
                         };

            return Request.CreateResponse(HttpStatusCode.OK, portal, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage PortalId(string alias)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetPortalIDByAlias(alias));
        }
        #endregion

        #region Administer Pages
        [HttpPost]
        public HttpResponseMessage GetViaPost(PageGetRequest page)
        {
            int myPortalID = ResolvePortalId(page.PortalID, page.PortalAlias, UserInfo.PortalID);
            int myTabID = ResolveTabId(page.ID, page.TabPath, page.TabFullName, myPortalID);

            if (myTabID == Null.NullInteger)
            { return ResponseNotFound(Resources.PageNotFoundWithIdOrPath, page.ID, page.TabPath); }

            var myTab = new TabController().GetTab(myTabID, myPortalID, ignoreCache: false);

            var tab = new PageGetResponse
            {
                TabID = myTab.TabID,
                TabName = myTab.TabName,
                TabPath = myTab.TabPath,
                ParentId = myTab.ParentId,
                PortalID = myTab.PortalID,
            };

            return Request.CreateResponse(HttpStatusCode.OK, tab, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage Get(string tabPath = null, string tabFullName = null, int? id = null,
                                       string portalAlias = null, int? portalID = null)
        {
            int myPortalID = ResolvePortalId(portalID, portalAlias, UserInfo.PortalID);
            int myTabID = ResolveTabId(id, tabPath, tabFullName, myPortalID);

            if (myTabID == Null.NullInteger)
            { return ResponseNotFound(Resources.PageNotFoundWithIdOrPath, id, tabPath); }

            var myTab = new TabController().GetTab(myTabID, myPortalID, ignoreCache: false);

            var tab = new PageGetResponse
            {
                TabID = myTab.TabID,
                TabName = myTab.TabName,
                TabPath = myTab.TabPath,
                ParentId = myTab.ParentId,
            };

            return Request.CreateResponse(HttpStatusCode.OK, tab, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage List(string portalAlias = "", int? portalID = null)
        {
            int myPortalID = ResolvePortalId(portalID, portalAlias, UserInfo.PortalID);

            List<TabInfo> listTabs = TabController.GetPortalTabs(myPortalID, Null.NullInteger,
                                                    includeNoneSpecified: false, noneSpecifiedText: null,
                                                    includeHidden: false, includeDeleted: false, includeURL: false,
                                                    checkViewPermisison: false, checkEditPermission: true);

            var tabs = (from tab in listTabs
                        select new PageListItemDto
                        {
                            TabID = tab.TabID,
                            TabName = tab.TabName,
                            Title = tab.Title,
                            TabPath = tab.TabPath,
                            PortalID = tab.PortalID,
                            ParentId = tab.ParentId,
                            IsDeleted = tab.IsDeleted,
                            IsVisible = tab.IsVisible,
                            IsSuperTab = tab.IsSuperTab,
                            Level = tab.Level,
                            ParentPath = (from tabParent in listTabs where tabParent.TabID == tab.ParentId select tabParent.TabPath).FirstOrDefault(),
                        }).Where(a => a.ParentId == Null.NullInteger || a.ParentPath != null);

            return Request.CreateResponse(HttpStatusCode.OK, tabs, new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage ListToDictionary(string portalAlias = "", int? portalID = null)
        {
            var tabs = PageListToDictionary(portalAlias, portalID);
            return Request.CreateResponse(HttpStatusCode.OK, tabs, new MediaTypeHeaderValue("text/json"));
        }

        [HttpPut]
        public HttpResponseMessage Import(string portalAlias = "", int? portalID = null,
                                          string parentPagePath = "", string beforePagePath = "", string afterPagePath = "",
                                          string parentPageFullName = "", string beforePageFullName = "", string afterPageFullName = "")
        {
            int myPortalID = ResolvePortalId(portalID, portalAlias, UserInfo.PortalID);

            var context = HttpContextSource.Current;

            string errorMessage = "";
            if (context.Request.Files.Count == 0) { errorMessage = Resources.NoTemplateFilesUploaded; }
            if (context.Request.Files.Count > 1) { errorMessage = Resources.OnlyOneTemplateFileAtATime; }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                                      errorMessage,
                                      new MediaTypeHeaderValue("text/json"));
            }

            //Get current Script time-out
            int scriptTimeout = context.Server.ScriptTimeout;
            try
            {
                //Set Script timeout to MAX value
                context.Server.ScriptTimeout = int.MaxValue;

                // save uploaded file to template folder
                var template = context.Request.Files[0];
                var templateFullPath = Path.Combine(GetTemplateFolder(), template.FileName);
                if (File.Exists(templateFullPath)) { File.Delete(templateFullPath); }
                template.SaveAs(templateFullPath);

                // import pages
                return InsertTabs(templateFullPath, myPortalID,
                                parentPagePath, beforePagePath, afterPagePath,
                                parentPageFullName, beforePageFullName, afterPageFullName);
            }
            // restore Script timeout
            finally { context.Server.ScriptTimeout = scriptTimeout; }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(PageGetRequest page, bool deleteDescendants = false)
        {
            int myPortalID = ResolvePortalId(page.PortalID, page.PortalAlias, UserInfo.PortalID);
            int myTabID = ResolveTabId(page.ID, page.TabPath, page.TabFullName, myPortalID);

            // page does not exist, return false (but do not generate error)
            if (myTabID == Null.NullInteger)
            { return Request.CreateResponse(HttpStatusCode.OK, false, new MediaTypeHeaderValue("text/json")); }

            var objTabs = new TabController();
            objTabs.DeleteTab(myTabID, myPortalID, deleteDescendants);
            // [2014-09-15] NOTE: HardDeleteTabInternal() call within is checking for child nodes after deleting them and before clearing the cache 
            // making the verification useless and keeping the page from being deleted when it has children 
            // On this call, only child pages get deleted and therefore an extra call is necessary to delete the page itself
            if (ResolveTabId(page.ID, page.TabPath, page.TabFullName, myPortalID) > Null.NullInteger)
            {
                if (deleteDescendants)
                { objTabs.DeleteTab(myTabID, myPortalID, deleteDescendants); }
                else
                { return ResponseNotFound(Resources.PageNotDeletedChildrenFound, page.ID, page.TabPath); }
            }
            return Request.CreateResponse(HttpStatusCode.OK, true, new MediaTypeHeaderValue("text/json"));
        }


        [HttpPost]
        public HttpResponseMessage Add(PageInfoDto page)
        {
            int myPortalID = ResolvePortalId(page.PortalID, page.PortalAlias, UserInfo.PortalID);

            var objTab = new TabInfo
            {
                PortalID = myPortalID,
                TabName = page.Name,
                Title = page.Title,
                Description = page.Description,
                Url = page.Url,
                IconFile = page.IconFile,
                IconFileLarge = page.IconFileLarge,
                IsVisible = true
            };

            objTab.TabSettings["AllowIndex"] = true.ToString();

            objTab.TabPermissions.Clear();
            // default permissions
            objTab.TabPermissions.AddRange(new TabPermissionCollection { 
                TabPermissionMapper.ToTabPermissionInfo(myPortalID, "Administrators", PagePermissionDto.KEY_VIEW),
                TabPermissionMapper.ToTabPermissionInfo(myPortalID, "Administrators", PagePermissionDto.KEY_EDIT),
            });
            // extra permissions
            if (page.Permissions != null)
            {
                foreach (var item in page.Permissions)
                { objTab.TabPermissions.Add(TabPermissionMapper.ToTabPermissionInfo(myPortalID, item)); }
            }

            var moduleCtrl = new dnn.ModuleController();
            var modules = new List<ModuleInfo>();
            HttpResponseMessage response = null;
            if (page.Modules != null)
            {
                foreach (var item in page.Modules)
                {
                    ModuleInfo module = null;
                    response = ModuleValidateAndAdd(item, ref module, modules);
                    if (response != null) { return response; }
                }
            }


            // get a list of all existing full page names in the form of a hashtable
            Hashtable tabs = null;
            if (!string.IsNullOrWhiteSpace(page.ParentPageFullName) || !string.IsNullOrWhiteSpace(page.BeforePageFullName) || !string.IsNullOrWhiteSpace(page.AfterPageFullName))
            { tabs = new Hashtable(PageListToDictionary(portalID: objTab.PortalID)); }

            // add page
            response = AddPage(objTab, page.ParentPagePath, page.BeforePagePath, page.AfterPagePath,
                                   tabs,
                                   page.ParentPageFullName, page.BeforePageFullName, page.AfterPageFullName);
            if (response != null) { return response; }

            // add modules to page
            foreach (var module in modules) { ModuleAddToPage(module, objTab, moduleCtrl); }

            return Request.CreateResponse(HttpStatusCode.OK, objTab.TabID, new MediaTypeHeaderValue("text/json"));
        }
        #endregion

        #region Private
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

        private HttpResponseMessage InsertTabs(string templateFullPath, int portalID,
                                               string parentPagePath, string beforePagePath, string afterPagePath,
                                               string parentPageFullName, string beforePageFullName, string afterPageFullName)
        {
            Hashtable modules = new Hashtable();
            var fileName = Path.GetFileName(templateFullPath);

            if (!IsAllowedExtension(fileName))
            { return ResponseNotFound(Resources.FileExtensionNotSupportedForFileName, fileName); }

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(templateFullPath);

            var tabNodes = new List<XmlNode>();
            var selectSingleNode = xmlDoc.SelectSingleNode("//portal/tabs");
            if (selectSingleNode != null)
            { tabNodes.AddRange(selectSingleNode.ChildNodes.Cast<XmlNode>()); }

            if (tabNodes.Count == 0)
            { return ResponseNotFound(Resources.NoPagesFoundInTemplate); }

            // get a list of all existing pages in the form of a hashtable
            var tabs = new Hashtable(PageListToDictionary(portalID: portalID));

            var objTabs = new TabController();

            //Create second tabs onwards. For first tab, we like to use tab details from input. 
            // For the rest (if any), it'll come from template
            for (var tabIndex = 0; tabIndex < tabNodes.Count; tabIndex++)
            {
                string tabName = null;
                try
                {
                    TabInfo objTab = null;
                    var tabNode = tabNodes[tabIndex];
                    tabName = XmlUtils.GetNodeValue(tabNode.CreateNavigator(), "name");

                    if (tabIndex == 0) // first page only
                    {
                        objTab = new TabInfo
                        {
                            TabName = tabName,
                            TabID = Null.NullInteger,
                            ParentId = Null.NullInteger,
                            PortalID = portalID,
                            IsVisible = true
                        };

                        // resolve parent (if not passed in)
                        if (string.IsNullOrWhiteSpace(parentPageFullName))
                        { parentPageFullName = XmlUtils.GetNodeValue(tabNode.CreateNavigator(), "parent"); }

                        // add-page
                        var response = AddPage(objTab, parentPagePath, beforePagePath, afterPagePath,
                                               tabs,
                                               parentPageFullName, beforePageFullName, afterPageFullName);
                        if (response != null) { return response; }
                    }

                    // make sure all properties are applied to pages added
                    TabController.DeserializeTab(tabNode, objTab, tabs, portalID, /*isAdminTemplate:*/ false, PortalTemplateModuleAction.Replace, modules);

                }
                catch (Exception ex)
                { return ResponseNotFound("ERROR: [{0}:{1}] {2}", tabIndex + 1, tabName, ex.Message); }
            }

            return Request.CreateResponse(HttpStatusCode.OK, tabNodes.Count, new MediaTypeHeaderValue("text/json"));
        }

        private HttpResponseMessage AddPage(TabInfo objTab,
                                            string parentPagePath, string beforePagePath, string afterPagePath,
                                            Hashtable tabs = null,
                                            string parentFullName = null, string beforePageFullName = null, string afterPageFullName = null)
        {
            var position = PositionEnum.AtEnd;
            string tabFullName = null;
            var positionTabId = Null.NullInteger;

            if (string.IsNullOrWhiteSpace(parentFullName))
            {
                if (!string.IsNullOrWhiteSpace(parentPagePath))
                {
                    objTab.ParentId = TabController.GetTabByTabPath(objTab.PortalID, parentPagePath, Null.NullString);
                    if (objTab.ParentId == Null.NullInteger)
                    { return ResponseNotFound(Resources.ParentPageNotFoundForTabName, parentPagePath, objTab.TabName); }
                }
            }
            else if (tabs != null && tabs[parentFullName] != null)
            {
                //parent node specifies the path (tab1/tab2/tab3), use saved tabid
                objTab.ParentId = Convert.ToInt32(tabs[parentFullName]);
                tabFullName = parentFullName + "/" + objTab.TabName;
            }

            // if parent path specified and not found...
            if (
                (!string.IsNullOrWhiteSpace(parentFullName) || !string.IsNullOrWhiteSpace(parentPagePath))
                && objTab.ParentId == Null.NullInteger
                )
            { return ResponseNotFound(Resources.ParentPageNotFoundForTabName, parentFullName, objTab.TabName); }


            // resolve BEFORE-page path
            if (string.IsNullOrWhiteSpace(beforePageFullName))
            {
                if (!string.IsNullOrWhiteSpace(beforePagePath))
                {
                    positionTabId = TabController.GetTabByTabPath(objTab.PortalID, beforePagePath, Null.NullString);
                    if (positionTabId == Null.NullInteger)
                    { return ResponseNotFound(Resources.BeforePagePathNotFoundForTabName, beforePagePath, objTab.TabName); }
                    position = PositionEnum.Before;
                }
            }
            else if (tabs != null && tabs[beforePageFullName] != null)
            {
                //parent node specifies the path (tab1/tab2/tab3), use saved tabid
                positionTabId = Convert.ToInt32(tabs[beforePageFullName]);
                if (positionTabId == Null.NullInteger)
                { return ResponseNotFound(Resources.BeforePageFullNameNotFoundForTabName, beforePageFullName, objTab.TabName); }
                position = PositionEnum.Before;
            }

            // resolve AFTER-page path
            if (string.IsNullOrWhiteSpace(afterPageFullName))
            {
                if (!string.IsNullOrWhiteSpace(afterPagePath))
                {
                    positionTabId = TabController.GetTabByTabPath(objTab.PortalID, afterPagePath, Null.NullString);
                    if (positionTabId == Null.NullInteger)
                    { return ResponseNotFound(Resources.AfterPagePathNotFoundForTabName, afterPagePath, objTab.TabName); }
                    position = PositionEnum.After;
                }
            }
            else if (tabs != null && tabs[afterPageFullName] != null)
            {
                //parent node specifies the path (tab1/tab2/tab3), use saved tabid
                positionTabId = Convert.ToInt32(tabs[afterPageFullName]);
                if (positionTabId == Null.NullInteger)
                { return ResponseNotFound(Resources.AfterPageFullNameNotFoundForTabName, afterPageFullName, objTab.TabName); }
                position = PositionEnum.After;
            }

            if (positionTabId > Null.NullInteger && objTab.ParentId == Null.NullInteger)
            {
                var objSiblingTab = new TabController().GetTab(positionTabId, objTab.PortalID, ignoreCache: false);
                if (objSiblingTab.ParentId > Null.NullInteger)
                { objTab.ParentId = objSiblingTab.ParentId; }
            }

            if (objTab.ParentId == Null.NullInteger) { tabFullName = objTab.TabName; }
            else if (tabs != null && string.IsNullOrWhiteSpace(tabFullName))
            {
                // build full name including parent full name
                foreach (DictionaryEntry entry in tabs)
                {
                    if (Convert.ToInt32(entry.Value) == objTab.ParentId)
                    {
                        tabFullName = entry.Key + "/" + objTab.TabName;
                        break;
                    }
                }
            }

            // build path
            objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            // verify if it does exist
            var tabId = TabController.GetTabByTabPath(objTab.PortalID, objTab.TabPath, Null.NullString);
            if (tabId != Null.NullInteger)
            { return ResponseNotFound(Resources.PageWithPathAlreadyExistsForTabName, objTab.TabPath, objTab.TabName); }

            // insert page
            switch (position)
            {
                case PositionEnum.AtEnd:
                    objTab.TabID = new TabController().AddTab(objTab);
                    break;
                case PositionEnum.Before:
                    objTab.TabID = new TabController().AddTabBefore(objTab, positionTabId);
                    break;
                case PositionEnum.After:
                    objTab.TabID = new TabController().AddTabAfter(objTab, positionTabId);
                    break;
                default: throw new NotImplementedException();
            }

            // add to hash table
            if (tabs != null) { tabs.Add(tabFullName, objTab.TabID); }
            return null;
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

        #endregion
    }
}