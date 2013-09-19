namespace BetterMembership.Utils
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;

    using CuttingEdge.Conditions;

    internal sealed class SqlQueryBuilder : ISqlQueryBuilder
    {
        private readonly string sqlProvider;

        private readonly SqlResourceFinder sqlResourceFinder;

        private readonly string userEmailColumn;

        private readonly string userIdColumn;

        private readonly string userNameColumn;

        private readonly string userTableName;

        private string deleteMembershipUserQuery;

        private string deleteOAuthMembershipUserQuery;

        private string deleteProfileQuery;

        private string deleteUserInRolesQuery;

        private string findUsersByEmailQuery;

        private string findUsersByNameQuery;

        private string getAllUsersQuery;

        private string getUserByIdQuery;

        private string getUserNameByEmailQuery;

        private string getUserProfileQuery;

        private string getUserQuery;

        private string unlockUserQuery;

        private string updateUserEmailQuery;

        private string updateUserMembershipQuery;

        public SqlQueryBuilder(
            SqlResourceFinder sqlResourceFinder, 
            string sqlProvider, 
            string userTableName, 
            string userIdColumn, 
            string userNameColumn, 
            string userEmailColumn)
        {
            Condition.Requires(sqlResourceFinder, "sqlResourceFinder").IsNotNull();
            Condition.Requires(sqlProvider, "sqlProvider").IsNotNullOrWhiteSpace();
            Condition.Requires(userTableName, "userTableName").IsNotNullOrWhiteSpace();
            Condition.Requires(userIdColumn, "userIdColumn").IsNotNullOrWhiteSpace();
            Condition.Requires(userNameColumn, "userNameColumn").IsNotNullOrWhiteSpace();

            this.sqlResourceFinder = sqlResourceFinder;
            this.sqlProvider = sqlProvider;
            this.userTableName = userTableName;
            this.userIdColumn = userIdColumn;
            this.userNameColumn = userNameColumn;
            this.userEmailColumn = userEmailColumn;
        }

        public string DeleteMembershipUser
        {
            get
            {
                return this.deleteMembershipUserQuery
                       ?? (this.deleteMembershipUserQuery = this.PrepareSqlStatment("sqlDeleteMembershipUser"));
            }
        }

        public string DeleteOAuthMembershipUser
        {
            get
            {
                return this.deleteOAuthMembershipUserQuery
                       ?? (this.deleteOAuthMembershipUserQuery = this.PrepareSqlStatment("sqlDeleteOAuthMembershipUser"));
            }
        }

        public string DeleteProfile
        {
            get
            {
                return this.deleteProfileQuery
                       ?? (this.deleteProfileQuery = this.PrepareSqlStatment("sqlDeleteProfile"));
            }
        }

        public string DeleteUserInRoles
        {
            get
            {
                return this.deleteUserInRolesQuery
                       ?? (this.deleteUserInRolesQuery = this.PrepareSqlStatment("sqlDeleteUserInRoles"));
            }
        }

        public string FindUsersByEmail
        {
            get
            {
                return this.findUsersByEmailQuery
                       ?? (this.findUsersByEmailQuery = this.PrepareSqlStatment("sqlFindUsersByEmail"));
            }
        }

        public string FindUsersByName
        {
            get
            {
                return this.findUsersByNameQuery
                       ?? (this.findUsersByNameQuery = this.PrepareSqlStatment("sqlFindUsersByName"));
            }
        }

        public string GetAllUsers
        {
            get
            {
                return this.getAllUsersQuery ?? (this.getAllUsersQuery = this.PrepareSqlStatment("sqlGetAllUsers"));
            }
        }

        public string GetUser
        {
            get
            {
                return this.getUserQuery ?? (this.getUserQuery = this.PrepareSqlStatment("sqlGetUser"));
            }
        }

        public string GetUserById
        {
            get
            {
                return this.getUserByIdQuery ?? (this.getUserByIdQuery = this.PrepareSqlStatment("sqlGetUserById"));
            }
        }

        public string GetUserNameByEmail
        {
            get
            {
                return this.getUserNameByEmailQuery
                       ?? (this.getUserNameByEmailQuery = this.PrepareSqlStatment("sqlGetUserNameByEmail"));
            }
        }

        public string GetUserProfile
        {
            get
            {
                return this.getUserProfileQuery
                       ?? (this.getUserProfileQuery = this.PrepareSqlStatment("sqlGetUserProfile"));
            }
        }

        public string UnlockUser
        {
            get
            {
                return this.unlockUserQuery ?? (this.unlockUserQuery = this.PrepareSqlStatment("sqlUnlockUser"));
            }
        }

        public string UpdateUserEmail
        {
            get
            {
                return this.updateUserEmailQuery
                       ?? (this.updateUserEmailQuery = this.PrepareSqlStatment("sqlUpdateUserEmail"));
            }
        }

        public string UpdateUserMembership
        {
            get
            {
                return this.updateUserMembershipQuery
                       ?? (this.updateUserMembershipQuery = this.PrepareSqlStatment("sqlUpdateUserMembership"));
            }
        }

        public string UpdateUserProfile(
            IEnumerable<SettingsPropertyValue> properties, string userName, out object[] values)
        {
            var builder = new StringBuilder("Update ").Append(this.userTableName).Append(" Set ");
            var sets = new List<string>();
            var propertyValues = new List<object>();
            var i = 1;
            propertyValues.Add(userName);
            foreach (SettingsPropertyValue property in properties)
            {
                sets.Add(string.Format("{0} = @{1}", property.Name, i));
                propertyValues.Add(property.PropertyValue);
                i++;
            }

            values = propertyValues.ToArray();
            builder.Append(string.Join(",", sets));
            builder.Append(" Where ").Append(this.userNameColumn).Append(" = @0");
            return builder.ToString();
        }

        private string PrepareSqlStatment(string sqlQueryName)
        {
            var sqlQuery = this.sqlResourceFinder.LocateScript(sqlQueryName, this.sqlProvider);

            return
                sqlQuery.Replace("[UserProfile]", this.userTableName)
                        .Replace("[userName]", this.userNameColumn)
                        .Replace("[userId]", this.userIdColumn)
                        .Replace("[email]", this.userEmailColumn ?? this.userNameColumn);
        }
    }
}