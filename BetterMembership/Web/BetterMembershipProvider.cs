namespace BetterMembership.Web
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

        public bool HasEmailColumnDefined
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.userEmailColumn);
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this.maxInvalidPasswordAttempts;
            }
        }

        public int PasswordLockoutTimeoutInSeconds
        {
            get
            {
                return this.passwordLockoutTimeoutInSeconds;
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
            Condition.Requires(email, "email").Evaluate(!(RequiresUniqueEmail && string.IsNullOrWhiteSpace(email)));

            var profile = new Dictionary<string, object>();

            if (this.RequiresUniqueEmail && this.HasEmailColumnDefined && !string.IsNullOrWhiteSpace(email))
            {
                var user = this.GetUserNameByEmail(email);
                if (user != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
            }

            if (this.HasEmailColumnDefined)
            {
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

            if (!this.HasEmailColumnDefined)
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

            if (userIsOnline)
            {
                throw new NotSupportedException("value provided for userIsOnline is not supported");
            }

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

            if (userIsOnline)
            {
                throw new NotSupportedException("value provided for userIsOnline is not supported");
            }

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

        public override string GetUserNameByEmail(string email)
        {
            Condition.Requires(email, "email").IsNotNullOrWhiteSpace();

            if (!this.HasEmailColumnDefined)
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

            if (config.ContainsKey("requiresQuestionAndAnswer"))
            {
                throw new ProviderException("unrecognized attribute requiresQuestionAndAnswer");
            }

            this.connectionStringName = config.GetString("connectionStringName", "DefaultConnection");
            this.userTableName = config.GetString("userTableName", "UserProfile");
            this.userIdColumn = config.GetString("userIdColumn", "UserId");
            this.userNameColumn = config.GetString("userNameColumn", "UserName");
            this.userEmailColumn = config.GetString("userEmailColumn");
            this.autoCreateTables = config.GetBoolean("autoCreateTables", true);
            var autoInitialize = config.GetBoolean("autoInitialize", true);
            this.maxInvalidPasswordAttempts = config.GetInteger("maxInvalidPasswordAttempts", int.MaxValue);
            this.passwordLockoutTimeoutInSeconds = config.GetInteger("passwordLockoutTimeoutInSeconds", int.MaxValue);
            this.requiresUniqueEmail = config.GetBoolean("requiresUniqueEmail");

            if (this.requiresUniqueEmail && !this.HasEmailColumnDefined)
            {
                throw new ProviderException("requiresUniqueEmail cannot be defined without userEmailColumn");
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
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string userName)
        {
            Condition.Requires(userName, "userName").IsNotNullOrWhiteSpace();

            using (var db = this.ConnectToDatabase())
            {
                return db.Execute(this.sqlHelper.UnlockUser, userName) > 0;
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            Condition.Requires(user, "user").IsNotNull();

            using (var db = this.ConnectToDatabase())
            {
                if (this.HasEmailColumnDefined)
                {
                    db.Execute(this.sqlHelper.UpdateUserWithEmail, user.UserName, user.Email, user.IsApproved);
                }
                else
                {
                    db.Execute(this.sqlHelper.UpdateUserWithoutEmail, user.UserName, user.IsApproved);
                }
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            return base.ValidateUser(username, password);
        }

        private static DateTime GetDateTime(object value)
        {
            return value == null ? DateTime.MinValue : (DateTime)value;
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
            DateTime lastPasswordFailureDate = GetDateTime(row[4]);

            int passwordFailuresSinceLastSuccess = row[5];
            DateTime creationDate = GetDateTime(row[6]);
            DateTime passwordChangedDate = GetDateTime(row[7]);
            string email = this.HasEmailColumnDefined ? row[8] : string.Empty;
            Func<bool> isLockedOutDelegate =
                () =>
                isConfirmed && passwordFailuresSinceLastSuccess > this.MaxInvalidPasswordAttempts
                && lastPasswordFailureDate.Add(TimeSpan.FromSeconds(this.PasswordLockoutTimeoutInSeconds))
                > DateTime.UtcNow;

            return new BetterMembershipUser(
                this.Name, 
                name, 
                userId, 
                email, 
                null, 
                null, 
                isConfirmed, 
                isLockedOutDelegate, 
                creationDate, 
                DateTime.MinValue, 
                DateTime.MinValue, 
                passwordChangedDate, 
                lastPasswordFailureDate, 
                this.HasEmailColumnDefined);
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