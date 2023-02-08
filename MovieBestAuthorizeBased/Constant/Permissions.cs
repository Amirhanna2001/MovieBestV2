using System;
using System.Collections.Generic;

namespace MovieBestAuthorizeBased.Constant
{
    public class Permissions
    {
        public static List<string> GeneratePermissionsList(string module)
        {
            return new List<string>(){
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }
        public static List<string> GenerateAllPermissions()
        {
            List<string> permissions = new();

            foreach (var module in Enum.GetValues(typeof(Modules)))
                permissions.AddRange(GeneratePermissionsList(module.ToString()));

            return permissions;
        }
    }
}
