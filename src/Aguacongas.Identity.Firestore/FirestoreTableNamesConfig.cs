using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.Identity.Firestore
{
    public class FirestoreTableNamesConfig
    {
        [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
        private struct Defaults
        {
            internal static readonly string UsersTableName = "users";
            internal static readonly string UserLoginsTableName = "user-logins";
            internal static readonly string UserClaimsTableName = "user-claims";
            internal static readonly string UserTokensTableName = "user-tokens";
            internal static readonly string RolesTableName = "roles";
            internal static readonly string RoleClaimsTableName = "role-claims";
            internal static readonly string UserRolesTableName = "users-roles";
        }

        public string UsersTableName { get; set; } = Defaults.UsersTableName;
        public string UserLoginsTableName {get; set; } = Defaults.UserLoginsTableName;
        public string UserClaimsTableName {get; set; } = Defaults.UserClaimsTableName;
        public string UserTokensTableName {get; set; } = Defaults.UserTokensTableName;
        public string RolesTableName {get; set; } = Defaults.RolesTableName;
        public string RoleClaimsTableName {get; set; } = Defaults.RoleClaimsTableName;
        public string UserRolesTableName {get; set; } = Defaults.UserRolesTableName;

        /// <summary>
        /// Adds suffix to every table names.
        /// </summary>
        /// <param name="suffix">suffix to add to every table name</param>
        public FirestoreTableNamesConfig WithSuffix(string suffix)
        {
            UsersTableName += suffix;
            UserLoginsTableName += suffix;
            UserClaimsTableName += suffix;
            UserTokensTableName += suffix;
            RolesTableName += suffix;
            RoleClaimsTableName += suffix;
            UserRolesTableName += suffix;
            return this;
        }
    }
}