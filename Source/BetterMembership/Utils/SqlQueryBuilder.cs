namespace BetterMembership.Utils
{
    using CuttingEdge.Conditions;

    internal sealed class SqlQueryBuilder : ISqlQueryBuilder
    {
        private readonly string sqlProvider;

        private readonly SqlResourceFinder sqlResourceFinder;

        private readonly string userEmailColumn;

        private readonly string userIdColumn;

        private readonly string userNameColumn;

        private readonly string userTableName;

        private string findUsersByEmailQuery;

        private string findUsersByNameQuery;

        private string getAllUsersQuery;

        private string getUserByIdQuery;

        private string getUserNameByEmailQuery;

        private string getUserQuery;

        private string unlockUserQuery;

        private string updateUserMembershipQuery;

        private string updateUserProfileQuery;

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

        public string FindUsersByEmailQuery
        {
            get
            {
                return this.findUsersByEmailQuery
                       ?? (this.findUsersByEmailQuery = this.PrepareSqlStatment("sqlFindUsersByEmail"));
            }
        }

        public string FindUsersByNameQuery
        {
            get
            {
                return this.findUsersByNameQuery
                       ?? (this.findUsersByNameQuery = this.PrepareSqlStatment("sqlFindUsersByName"));
            }
        }

        public string GetAllUsersQuery
        {
            get
            {
                return this.getAllUsersQuery ?? (this.getAllUsersQuery = this.PrepareSqlStatment("sqlGetAllUsers"));
            }
        }

        public string GetUserByIdQuery
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

        public string GetUserQuery
        {
            get
            {
                return this.getUserQuery ?? (this.getUserQuery = this.PrepareSqlStatment("sqlGetUser"));
            }
        }

        public string UnlockUser
        {
            get
            {
                return this.unlockUserQuery ?? (this.unlockUserQuery = this.PrepareSqlStatment("sqlUnlockUser"));
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

        public string UpdateUserProfile
        {
            get
            {
                return this.updateUserProfileQuery
                       ?? (this.updateUserProfileQuery = this.PrepareSqlStatment("sqlUpdateUserProfile"));
            }
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