﻿using System;
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
        public const string User = "User";

        public const string AccessSysadmin = Sysadmin;
        public const string AccessAdmin = AccessSysadmin + "," + Admin;
        public const string AccessUser = AccessAdmin + "," + User;


        public static string[] AvailableRoles = new string[] {RoleStrings.HumanResource, RoleStrings.User};
    }
}
