namespace BetterMembership
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Web.Security;

    using BetterMembership.Data;
    using BetterMembership.Extensions;
    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using global::BetterMembershipProvider.Utils;

    using CuttingEdge.Conditions;

    using WebMatrix.Data;
    using WebMatrix.WebData;

    public sealed class BetterMembershipProvider : SimpleMembershipProvider
    {
        private readonly Func<string, IDatabase> databaseFactory;

        private readonly Func<string, string, string, string, SqlHelper> sqlHelperFactory;

        private readonly WebSecurityFacade webSecurityFacade;

        private bool autoCreateTables;

        private string connectionStringName;

        private int maxInvalidPasswordAttempts;

        private int passwordLockoutTimeoutInSeconds;

        private bool requiresUniqueEmail;

        private SqlHelper sqlHelper;

        private string userEmailColumn;

        private string userIdColumn;

        private string userNameColumn;

        private string userTableName;

        public BetterMembershipProvider()
            : this(
                new WebSecurityFacade(), 
                s => new DatabaseProxy(Database.Open(s)), 
                (a, b, c, d) => new SqlHelper(a, b, c, d))
        {
        }

        internal BetterMembershipProvider(
            WebSecurityFacade webSecurityFacade, 
            Func<string, IDatabase> databaseFactory, 
            Func<string, string, string, string, SqlHelper> sqlHelperFactory)
        {
            Condition.Requires(webSecurityFacade, "webSecurityFacade").IsNotNull();
            Condition.Requires(databaseFactory, "databaseFactory").IsNotNull();
            Condition.Requires(sqlHelperFactory, "sqlHelperFactory").IsNotNull();

            this.webSecurityFacade = webSecurityFacade;
            this.databaseFactory = databaseFactory;
            this.sqlHelperFactory = sqlHelperFactory;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this.maxInvalidPasswordAttempts;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return this.requiresUniqueEmail;
            }
        }

        private bool HasEmailColumnDefined
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.userEmailColumn);
            }
        }

        public override MembershipUser CreateUser(
            string username, 
            string password, 
            string email, 
            string passwordQuestion, 
            string passwordAnswer, 
            bool isApproved, 
            object providerUserKey, 
            out MembershipCreateStatus status)
        {
            var profile = new Dictionary<string, object>();

            if (this.RequiresUniqueEmail && this.HasEmailColumnDefined && !string.IsNullOrWhiteSpace(email))
            {
                var user = this.GetUserNameByEmail(email);
                if (string.Compare(user, username, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }

                profile.Add(this.userEmailColumn, email);
            }

            try
            {
                this.CreateUserAndAccount(username, password, isApproved, profile);
            }
            catch (MembershipCreateUserException e)
            {
                status = e.StatusCode;
                return null;
            }

            status = MembershipCreateStatus.Success;
            return GetUser(username, false);
        }

        public override MembershipUserCollection FindUsersByEmail(
            string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            Condition.Requires(emailToMatch, "emailToMatch").IsNotNullOrWhiteSpace();
            Condition.Requires(pageIndex, "pageIndex").IsNotLessThan(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            if (!HasEmailColumnDefined)
            {
                throw new ProviderException("userEmailColumn is not defined");
            }

            var startRow = GetPagingStartRow(pageIndex, pageSize);
            var endRow = GetPagingEndRow(pageSize, startRow);

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlHelper.FindUsersByEmailQuery, startRow, endRow, emailToMatch).ToList();
                return this.ExtractMembershipUsersFromRows(rows, out totalRecords);
            }
        }

        public override MembershipUserCollection FindUsersByName(
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            Condition.Requires(usernameToMatch, "usernameToMatch").IsNotNullOrWhiteSpace();
            Condition.Requires(pageIndex, "pageIndex").IsNotLessThan(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            var startRow = GetPagingStartRow(pageIndex, pageSize);
            var endRow = GetPagingEndRow(pageSize, startRow);

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlHelper.FindUsersByNameQuery, startRow, endRow, usernameToMatch).ToList();
                return this.ExtractMembershipUsersFromRows(rows, out totalRecords);
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            Condition.Requires(pageIndex, "pageIndex").IsNotLessThan(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            var startRow = GetPagingStartRow(pageIndex, pageSize);
            var endRow = GetPagingEndRow(pageSize, startRow);

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlHelper.GetAllUsersQuery, startRow, endRow).ToList();
                return this.ExtractMembershipUsersFromRows(rows, out totalRecords);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            Condition.Requires(username, "username").IsNotNullOrWhiteSpace();

            using (var db = this.ConnectToDatabase())
            {
                var row = db.QuerySingle(this.sqlHelper.GetUserQuery, username);
                if (row != null)
                {
                    return this.CreateMembershipUser(row);
                }
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Condition.Requires(providerUserKey, "providerUserKey").IsNotNull();
            Condition.Requires(providerUserKey, "providerUserKey").IsOfType(typeof(int));

            var userId = (int)providerUserKey;

            using (var db = this.ConnectToDatabase())
            {
                var row = db.QuerySingle(this.sqlHelper.GetUserByIdQuery, userId);
                if (row != null)
                {
                    return this.CreateMembershipUser(row);
                }
            }

            return null;
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            return base.GetUserIdFromPasswordResetToken(token);
        }

        public override string GetUserNameByEmail(string email)
        {
            Condition.Requires(email, "email").IsNotNullOrWhiteSpace();

            if (!HasEmailColumnDefined)
            {
                throw new ProviderException("userEmailColumn is not defined");
            }

            using (var db = this.ConnectToDatabase())
            {
                return db.QueryValue(this.sqlHelper.GetUserNameByEmail, email) as string;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();
            Condition.Requires(config, "config").IsNotNull();

            this.connectionStringName = config.GetString("connectionStringName", "DefaultConnection");
            this.userTableName = config.GetString("userTableName", "UserProfile");
            this.userIdColumn = config.GetString("userIdColumn", "UserId");
            this.userNameColumn = config.GetString("userNameColumn", "UserName");
            this.userEmailColumn = config.GetString("userEmailColumn");
            this.autoCreateTables = config.GetBoolean("autoCreateTables", true);
            var autoInitialize = config.GetBoolean("autoInitialize", true);
            this.maxInvalidPasswordAttempts = config.GetInteger("maxInvalidPasswordAttempts", int.MaxValue);
            this.passwordLockoutTimeoutInSeconds = config.GetInteger("passwordLockoutTimeoutInSeconds", int.MaxValue);
            this.requiresUniqueEmail = config.GetBoolean("requiresUniqueEmail", false);

            if (this.requiresUniqueEmail && !this.HasEmailColumnDefined)
            {
                throw new ConfigurationErrorsException("requiresUniqueEmail cannot be defined without userEmailColumn");
            }

            config.Remove("userTableName");
            config.Remove("userIdColumn");
            config.Remove("userNameColumn");
            config.Remove("userEmailColumn");
            config.Remove("autoCreateTables");
            config.Remove("autoInitialize");
            config.Remove("passwordLockoutTimeoutInSeconds");

            this.sqlHelper = this.sqlHelperFactory(
                this.userTableName, this.userIdColumn, this.userNameColumn, this.userEmailColumn);

            base.Initialize(name, config);

            if (autoInitialize)
            {
                this.InitializeDatabaseConnection();
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            return base.ResetPassword(username, answer);
        }

        public override bool UnlockUser(string userName)
        {
            return base.UnlockUser(userName);
        }

        public override void UpdateUser(MembershipUser user)
        {
            }

        private static int GetPagingEndRow(int pageSize, int startRow)
        {
            return (startRow + pageSize) - 1;
        }

        private static int GetPagingStartRow(int pageIndex, int pageSize)
        {
            return (pageIndex * pageSize) + 1;
        }

        private static int GetTotalRecords(IList<dynamic> rows)
        {
            return (int)rows[0][0];
        }

        private IDatabase ConnectToDatabase()
        {
            return this.databaseFactory(this.connectionStringName);
        }

        private MembershipUser CreateMembershipUser(dynamic row)
        {
            int userId = row[1];
            string name = row[2];
            bool isConfirmed = row[3];
            DateTime lastPasswordFailureDate = this.GetDateTime(row[4]);
            int passwordFailuresSinceLastSuccess = row[5];
            DateTime creationDate = this.GetDateTime(row[6]);
            DateTime passwordChangedDate = this.GetDateTime(row[7]);
            string email = HasEmailColumnDefined ? row[8] : string.Empty;

            bool isLockedOut = passwordFailuresSinceLastSuccess > this.MaxInvalidPasswordAttempts
                               && lastPasswordFailureDate.Add(
                                   TimeSpan.FromSeconds(this.passwordLockoutTimeoutInSeconds)) > DateTime.UtcNow;

            return new MembershipUser(
                this.Name, 
                name, 
                userId, 
                email, 
                null, 
                null, 
                isConfirmed, 
                isLockedOut, 
                creationDate, 
                DateTime.MinValue, 
                DateTime.MinValue, 
                passwordChangedDate, 
                lastPasswordFailureDate);
        }

        private MembershipUserCollection ExtractMembershipUsersFromRows(List<dynamic> rows, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            if (rows.Any())
            {
                totalRecords = GetTotalRecords(rows);
                rows.ForEach(row => users.Add(this.CreateMembershipUser(row)));
            }

            return users;
        }

        private DateTime GetDateTime(object value)
        {
            return value == null ? DateTime.MinValue : (DateTime)value;
        }

        private void InitializeDatabaseConnection()
        {
            using (new DefaultProviderSwitcher(this))
            {
                this.webSecurityFacade.InitializeDatabaseConnection(
                    this.connectionStringName, 
                    this.userTableName, 
                    this.userIdColumn, 
                    this.userNameColumn, 
                    this.autoCreateTables);
            }
        }
    }
}