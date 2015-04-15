using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Web.Api;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Build.DotNetNuke.Deployer.Services
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class SettingsController : AdminBaseController
    {

        #region Public
        [HttpGet]
        public new HttpResponseMessage GetTemplateFolder()
        {
            return Request.CreateResponse(HttpStatusCode.OK, base.GetTemplateFolder(), new MediaTypeHeaderValue("text/json"));
        }

        [HttpGet]
        public HttpResponseMessage GetModuleDefinitionByFriendlyName(string friendlyName)
        {
            var mi = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(friendlyName);
            var results = new
            {
                DesktopModuleID = mi.DesktopModuleID,
                ModuleDefID = mi.ModuleDefID,
                DefinitionName = mi.DefinitionName,
                FriendlyName = mi.FriendlyName,
                KeyID = mi.KeyID,
            };
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        [HttpGet]
        public HttpResponseMessage GetPublicRoles()
        {
            PortalSettings ps = PortalController.GetCurrentPortalSettings();
            var rc = new RoleController();
            ArrayList listRoles = rc.GetPortalRoles(ps.PortalId);

            var results = (from RoleInfo role in listRoles
                           where role.IsPublic
                           select new
                           {
                               RoleId = role.RoleID,
                               RoleName = role.RoleName,
                               Subscribed = UserInfo.IsInRole(role.RoleName)
                           }).OrderBy(sr => sr.RoleName).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
        #endregion

        #region Private
        #endregion

    }
}