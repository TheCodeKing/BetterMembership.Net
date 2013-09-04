namespace BetterMembershipProvider.Utils
{
    using BetterMembershipProvider.Properties;

    using CuttingEdge.Conditions;

    internal sealed class SqlHelper
    {
        private readonly string userEmailColumn;

        private readonly string userIdColumn;

        private readonly string userNameColumn;

        private readonly string userTableName;

        private string getUserQuery;

        private string getAllUsersQuery;

        private string findUsersByNameQuery;

        private string findUsersByEmailQuery;

        private string getUserByIdQuery;

        private string getUserNameByEmailQuery;

        public SqlHelper(string userTableName, string userIdColumn, string userNameColumn, string userEmailColumn)
        {
            Condition.Requires(userTableName, "userTableName").IsNotNullOrWhiteSpace();
            Condition.Requires(userIdColumn, "userIdColumn").IsNotNullOrWhiteSpace();
            Condition.Requires(userNameColumn, "userNameColumn").IsNotNullOrWhiteSpace();

            this.userTableName = userTableName;
            this.userIdColumn = userIdColumn;
            this.userNameColumn = userNameColumn;
            this.userEmailColumn = userEmailColumn;
        }

        public string GetUserQuery
        {
            get
            {
                return this.getUserQuery ?? (this.getUserQuery = this.PrepareSqlStatment(Resources.SqlGetUser));
            }
        }

        public string GetAllUsersQuery
        {
            get
            {
                return this.getAllUsersQuery ?? (this.getAllUsersQuery = this.PrepareSqlStatment(Resources.SqlGetAllUsers)); 
            }
        }

        public string FindUsersByNameQuery
        {
            get
            {
                return this.findUsersByNameQuery ?? (this.findUsersByNameQuery = this.PrepareSqlStatment(Resources.SqlFindUsersByName)); 
            }
        }

        public string FindUsersByEmailQuery
        {
            get
            {
                return this.findUsersByEmailQuery
                       ?? (this.findUsersByEmailQuery = this.PrepareSqlStatment(Resources.SqlFindUsersByEmail));
            }
        }

        public string GetUserByIdQuery
        {
            get
            {
                return this.getUserByIdQuery
                       ?? (this.getUserByIdQuery = this.PrepareSqlStatment(Resources.sqlGetUserById));
            }
        }

        public string GetUserNameByEmail
        {
            get
            {
                return this.getUserNameByEmailQuery
                       ?? (this.getUserNameByEmailQuery = this.PrepareSqlStatment(Resources.sqlGetUserNameByEmail));
            }
        }

        private string PrepareSqlStatment(string sqlQuery)
        {
            return sqlQuery.Replace("[UserProfile]", this.userTableName)
                          .Replace("[userName]", this.userNameColumn)
                          .Replace("[userId]", this.userIdColumn)
                          .Replace("[email]", this.userEmailColumn ?? userNameColumn);
        }
    }
}