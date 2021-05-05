namespace Aguacongas.Identity.Firestore
{
    public class FirestoreTableNamesConfig
    {
        private struct Defaults
        {
            internal const string UsersTableName = "users";
            internal const string UserLoginsTableName = "user-logins";
            internal const string UserClaimsTableName = "user-claims";
            internal const string UserTokensTableName = "user-tokens";
            internal const string RolesTableName = "roles";
            internal const string RoleClaimsTableName = "role-claims";
            internal const string UserRolesTableName = "users-roles";
        }

        public string UsersTableName { get; set; } = Defaults.UsersTableName;
        public string UserLoginsTableName {get; set; } = Defaults.UserLoginsTableName;
        public string UserClaimsTableName {get; set; } = Defaults.UserClaimsTableName;
        public string UserTokensTableName {get; set; } = Defaults.UserTokensTableName;
        public string RolesTableName {get; set; } = Defaults.RolesTableName;
        public string RoleClaimsTableName {get; set; } = Defaults.RoleClaimsTableName;
        public string UserRolesTableName {get; set; } = Defaults.UserRolesTableName;
    }
}