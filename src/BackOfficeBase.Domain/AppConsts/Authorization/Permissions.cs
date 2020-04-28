using System.Collections.Generic;

namespace BackOfficeBase.Domain.AppConsts.Authorization
{
    public static class AppPermissions
    {
        public static List<string> GetAll()
        {
            return new List<string>
            {
                Users.Read, Users.Create, Users.Update, Users.Delete,
                Roles.Read, Roles.Create, Roles.Update, Roles.Delete,
                OrganizationUnits.Read, OrganizationUnits.Create, OrganizationUnits.Update, OrganizationUnits.Delete
            };
        }

        public static class Users
        {
            public const string Read = "Permissions.Users.Read";
            public const string Create = "Permissions.Users.Create";
            public const string Update = "Permissions.Users.Update";
            public const string Delete = "Permissions.Users.Delete";
        }

        public static class Roles
        {
            public const string Read = "Permissions.Roles.Read";
            public const string Create = "Permissions.Roles.Create";
            public const string Update = "Permissions.Roles.Update";
            public const string Delete = "Permissions.Roles.Delete";
        }

        public static class OrganizationUnits
        {
            public const string Read = "Permissions.OrganizationUnits.Read";
            public const string Create = "Permissions.OrganizationUnits.Create";
            public const string Update = "Permissions.OrganizationUnits.Update";
            public const string Delete = "Permissions.OrganizationUnits.Delete";
        }
    }
}
