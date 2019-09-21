using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_WebServer.Entities
{

    public static class RoleStrings
    {
        public const string Sysadmin = "Sysadmin";
        public const string Admin = "Admin";
        public const string HumanResource = "HumanResource";
        public const string It = "It";
        public const string User = "User";

        public const string AccessSysadmin = Sysadmin;
        public const string AccessAdmin = AccessSysadmin + "," + Admin;
        public const string AccessIt = It + "," + AccessAdmin;
        public const string AccessHumanResource = HumanResource + "," + AccessAdmin;
        public const string AccessUser = AccessAdmin + "," + User;


        public static string[] AvailableRoles = new string[]
        {
            RoleStrings.HumanResource,
            RoleStrings.It,
            //RoleStrings.User
        };
        public static string[] AvailableRolesAdmin = new string[]
        {
            RoleStrings.HumanResource,
            RoleStrings.It,
            RoleStrings.Admin
        };
        public static string[] AvailableRolesSysadmin = new string[]
        {
            RoleStrings.HumanResource,
            RoleStrings.It,
            RoleStrings.Admin,
            RoleStrings.Sysadmin,
        };
    }
}
