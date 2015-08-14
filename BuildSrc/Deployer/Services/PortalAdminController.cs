using Build.DotNetNuke.Deployer.Library;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Build.DotNetNuke.Deployer.Services
{
    [RequireHost]
    public class PortalAdminController : AdminBaseController
    {
        protected override List<string> AllowedExtensionsLowerCase { get { return new List<string> { ".zip", ".template" }; } }


        #region Get Portal Information
        [HttpGet]
        public HttpResponseMessage List()
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
        public HttpResponseMessage Get(string alias = "", int? id = null)
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
        public HttpResponseMessage FindId(string alias)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetPortalIDByAlias(alias));
        }
        #endregion

        #region Administer Pages


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

                throw new NotImplementedException();
            }
            // restore Script timeout
            finally { context.Server.ScriptTimeout = scriptTimeout; }
        }

        /*
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
            if (page.Modules != null)
            {
                foreach (var item in page.Modules)
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

                    var module = new ModuleInfo
                    {
                        ModuleDefID = moduleDefinition.ModuleDefID,
                        //DesktopModuleID = desktopModule.DesktopModuleID,
                        //ModuleID = desktopModule.ModuleID,
                        //
                        ModuleTitle = item.ModuleName,
                        PaneName = item.PaneName,
                        //
                        InheritViewPermissions = true,
                    };
                    // add module
                    modules.Add(module);
                }
            }

            // TODO: implemenentation not done
            throw new NotImplementedException();

            // add portal

            return Request.CreateResponse(HttpStatusCode.OK, objTab.TabID, new MediaTypeHeaderValue("text/json"));
        }
         */
        #endregion

        #region Private
        private int GetPortalIDByAlias(string portalAlias)
        {
            var portalAliasInfo = TestablePortalAliasController.Instance.GetPortalAlias(portalAlias);
            if (portalAliasInfo == null) { throw new ArgumentOutOfRangeException("portalAlias", string.Format(Resources.PortalAliasNotFound, portalAlias)); }
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

        #endregion
    }
}