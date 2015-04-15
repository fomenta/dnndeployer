using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;
using System;
using System.Collections;

namespace Build.DotNetNuke.Deployer.Library
{
    public class TabPermissionMapper
    {
        public const string DEFAULT_CODE = "SYSTEM_TAB";

        #region Class
        public string PermissionKey { get; set; }
        public string RoleName { get; set; }

        public TabPermissionMapper(string RoleName, string PermissionKey)
        {
            this.RoleName = RoleName;
            this.PermissionKey = PermissionKey;
        }

        public TabPermissionInfo ToTabPermissionInfo(int portalID)
        {
            var p = new TabPermissionInfo
            {
                PermissionID = GetPermissionId(DEFAULT_CODE, PermissionKey),
                RoleID = GetRoleId(portalID, RoleName),
                UserID = Null.NullInteger,
                AllowAccess = true,
            };

            return p;
        }
        #endregion

        #region Static

        public static TabPermissionInfo ToTabPermissionInfo(int portalID, PagePermissionDto dto)
        {
            return new TabPermissionMapper(dto.RoleName, dto.PermissionKey).ToTabPermissionInfo(portalID);
        }

        public static TabPermissionInfo ToTabPermissionInfo(int portalID, string RoleName, string PermissionKey)
        {
            return new TabPermissionMapper(RoleName, PermissionKey).ToTabPermissionInfo(portalID);
        }

        private static int GetPermissionId(string permissionCode, string permissionKey)
        {
            ArrayList arrPermissions = new PermissionController().GetPermissionByCodeAndKey(permissionCode, permissionKey);
            int permissionID = 0;
            int i;
            for (i = 0; i <= arrPermissions.Count - 1; i++)
            {
                var permission = (PermissionInfo)arrPermissions[i];
                permissionID = permission.PermissionID;
            }
            return permissionID;
        }

        private static int GetRoleId(int portalID, string roleName)
        {
            int roleID = int.MinValue;
            switch (roleName)
            {
                case Globals.glbRoleAllUsersName:
                    roleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                    break;
                case Globals.glbRoleUnauthUserName:
                    roleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                    break;
                default:
                    var portalController = new PortalController();
                    PortalInfo portal = portalController.GetPortal(portalID);
                    RoleInfo role = TestableRoleController.Instance.GetRole(portal.PortalID,
                                                                            r => r.RoleName == roleName);
                    if (role != null) { roleID = role.RoleID; }
                    else
                    {
                        if (roleName.ToLower() == "administrators") { roleID = portal.AdministratorRoleId; }
                    }
                    break;
            }
            return roleID;
        }
        #endregion
    }
}