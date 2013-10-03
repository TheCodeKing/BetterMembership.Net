namespace BetterMembership.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Helpers;
    using System.Web.Security;

    using BetterMembership.Data;
    using BetterMembership.Extensions;
    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using CuttingEdge.Conditions;

    using WebMatrix.WebData;

    using Database = WebMatrix.Data.Database;

    public sealed class BetterMembershipProvider : SimpleMembershipProvider
    {
        private const int PasswordSize = 14;

        private readonly Func<string, IDatabase> databaseFactory;

        private readonly Func<string, string, string, string, string, ISqlQueryBuilder> sqlQueryBuilderFactory;

        private readonly IWebSecurityFacade webSecurityFacade;

        private bool autoCreateTables;

        private bool autoInitialize;

        private string connectionStringName;

        private string emailStrengthRegularExpression;

        private int maxEmailLength;

        private int maxInvalidPasswordAttempts;

        private int maxPasswordLength;

        private int maxUserNameLength;

        private int minRequiredNonalphanumericCharacters;

        private int minRequiredPasswordLength;

        private int passwordAttemptWindowInSeconds;

        private bool requiresUniqueEmail;

        private ISqlQueryBuilder sqlQueryBuilder;

        private string userEmailColumn;

        private string userIdColumn;

        private string userNameColumn;

        private string userNameRegularExpression;

        private string userTableName;

        private bool allowEmailAsUserName;

        public BetterMembershipProvider()
            : this(
                new WebSecurityFacade(), 
                s => new DatabaseProxy(Database.Open(s)), 
                (a, b, c, d, e) =>
                new SqlQueryBuilder(
                    new SqlResourceFinder(new ResourceManifestFacade(typeof(BetterMembershipProvider).Assembly)), 
                    a, 
                    b, 
                    c, 
                    d, 
                    e))
        {
        }

        internal BetterMembershipProvider(
            IWebSecurityFacade webSecurityFacade, 
            Func<string, IDatabase> databaseFactory, 
            Func<string, string, string, string, string, ISqlQueryBuilder> sqlQueryBuilderFactory)
        {
            Condition.Requires(webSecurityFacade, "webSecurityFacade").IsNotNull();
            Condition.Requires(databaseFactory, "databaseFactory").IsNotNull();
            Condition.Requires(sqlQueryBuilderFactory, "sqlQueryBuilderFactory").IsNotNull();

            this.webSecurityFacade = webSecurityFacade;
            this.databaseFactory = databaseFactory;
            this.sqlQueryBuilderFactory = sqlQueryBuilderFactory;
        }

        public override string ApplicationName { get; set; }

        public string EmailStrengthRegularExpression
        {
            get
            {
                return this.emailStrengthRegularExpression;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }

        public bool HasEmailColumnDefined
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.userEmailColumn);
            }
        }

        public int MaxEmailLength
        {
            get
            {
                return this.maxEmailLength;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this.maxInvalidPasswordAttempts;
            }
        }

        public int MaxPasswordLength
        {
            get
            {
                return this.maxPasswordLength;
            }
        }

        public int MaxUserNameLength
        {
            get
            {
                return this.maxUserNameLength;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return this.minRequiredNonalphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return this.minRequiredPasswordLength;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return this.passwordAttemptWindowInSeconds / 60;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return MembershipPasswordFormat.Hashed;
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

        public string UserEmailColumn
        {
            get
            {
                return this.userEmailColumn;
            }
        }

        public string UserNameRegularExpression
        {
            get
            {
                return this.userNameRegularExpression;
            }
        }

        internal bool AutoCreateTables
        {
            get
            {
                return this.autoCreateTables;
            }
        }

        internal bool AutoInitialize
        {
            get
            {
                return this.autoInitialize;
            }
        }

        internal string ConnectionStringName
        {
            get
            {
                return this.connectionStringName;
            }
        }

        internal int PasswordAttemptWindowInSeconds
        {
            get
            {
                return this.passwordAttemptWindowInSeconds;
            }
        }

        public bool AllowEmailAsUserName
        {
            get
            {
                return this.allowEmailAsUserName;
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Condition.Requires(username, "username").IsNotNullOrWhiteSpace();
            Condition.Requires(oldPassword, "oldPassword").IsNotNullOrWhiteSpace();

            Assert(this.ValidatePassword(newPassword, username), MembershipCreateStatus.InvalidPassword);

            return base.ChangePassword(username, oldPassword, newPassword);
        }

        public override bool ChangePasswordQuestionAndAnswer(
            string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            Assert(this.ValidateUserName(userName), MembershipCreateStatus.InvalidUserName);
            Assert(this.ValidatePassword(password, userName), MembershipCreateStatus.InvalidPassword);

            return base.CreateAccount(userName, password, requireConfirmationToken);
        }

        public override string CreateAccount(string userName, string password)
        {
            Assert(this.ValidateUserName(userName), MembershipCreateStatus.InvalidUserName);
            Assert(this.ValidatePassword(password, userName), MembershipCreateStatus.InvalidPassword);

            return base.CreateAccount(userName, password);
        }

        public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
        {
            Condition.Ensures(provider, "provider").IsNotNullOrWhiteSpace();
            Condition.Ensures(providerUserId, "providerUserId").IsNotNullOrWhiteSpace();
            Condition.Ensures(userName, "userName").IsNotNullOrWhiteSpace();

            base.CreateOrUpdateOAuthAccount(provider, providerUserId, userName);
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
            Condition.Requires(passwordQuestion, "passwordQuestion").IsNullOrWhiteSpace("not supported, expected null");
            Condition.Requires(passwordAnswer, "passwordAnswer").IsNullOrWhiteSpace("not supported, expected null");

            if (!this.ValidateUserName(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (!this.ValidatePassword(password, username))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (!this.ValidateProviderUserKey(providerUserKey, true))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            if (!this.ValidateEmail(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            var profile = new Dictionary<string, object>();

            if (!this.HasEmailColumnDefined && !string.IsNullOrWhiteSpace(email))
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }

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
                this.CreateUserAndAccount(username, password, !isApproved, profile);
            }
            catch (MembershipCreateUserException e)
            {
                status = e.StatusCode;
                return null;
            }

            status = MembershipCreateStatus.Success;
            return GetUser(username, false);
        }

        public override string CreateUserAndAccount(
            string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            Assert(this.ValidateUserName(userName), MembershipCreateStatus.InvalidUserName);
            Assert(this.ValidatePassword(password, userName), MembershipCreateStatus.InvalidPassword);

            if (this.RequiresUniqueEmail && (values == null || !values.ContainsKey(this.UserEmailColumn)))
            {
                throw new MembershipCreateUserException(
                    string.Format("{0} is required", this.UserEmailColumn), 
                    new MembershipCreateUserException(MembershipCreateStatus.UserRejected));
            }

            return base.CreateUserAndAccount(userName, password, requireConfirmation, values);
        }

        public override string CreateUserAndAccount(
            string userName, string password, IDictionary<string, object> values)
        {
            return this.CreateUserAndAccount(userName, password, false, values);
        }

        public override string CreateUserAndAccount(string userName, string password)
        {
            return this.CreateUserAndAccount(userName, password, false, null);
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation)
        {
            return this.CreateUserAndAccount(userName, password, requireConfirmation, null);
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

            using (var db = this.ConnectToDatabase())
            {
                emailToMatch = AppendWildcardToSearchTerm(emailToMatch);
                var rows = db.Query(this.sqlQueryBuilder.FindUsersByEmail, startRow, pageSize, emailToMatch).ToList();
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

            using (var db = this.ConnectToDatabase())
            {
                usernameToMatch = AppendWildcardToSearchTerm(usernameToMatch);
                var rows = db.Query(this.sqlQueryBuilder.FindUsersByName, startRow, pageSize, usernameToMatch).ToList();
                return this.ExtractMembershipUsersFromRows(rows, out totalRecords);
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            Condition.Requires(pageIndex, "pageIndex").IsNotLessThan(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            var startRow = GetPagingStartRow(pageIndex, pageSize);

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlQueryBuilder.GetAllUsers, startRow, pageSize).ToList();
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
                var row = db.QuerySingle(this.sqlQueryBuilder.GetUser, username);
                if (row != null)
                {
                    return this.CreateMembershipUser(row);
                }
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Condition.Requires(providerUserKey, "providerUserKey").IsOfType(typeof(int));
            Condition.Requires((int)providerUserKey, "providerUserKey").IsGreaterThan(-1);

            var userId = (int)providerUserKey;

            using (var db = this.ConnectToDatabase())
            {
                var row = db.QuerySingle(this.sqlQueryBuilder.GetUserById, userId);
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
                return db.QueryValue(this.sqlQueryBuilder.GetUserNameByEmail, email) as string;
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

            if (config.ContainsKey("enablePasswordRetrieval"))
            {
                throw new ProviderException("unrecognized attribute enablePasswordRetrieval");
            }

            if (config.ContainsKey("enablePasswordReset"))
            {
                throw new ProviderException("unrecognized attribute enablePasswordReset");
            }

            if (config.ContainsKey("passwordFormat"))
            {
                throw new ProviderException("unrecognized attribute passwordFormat");
            }

            this.connectionStringName = config.GetString("connectionStringName", "DefaultConnection");
            this.userTableName = config.GetString("userTableName", "UserProfile");
            this.userIdColumn = config.GetString("userIdColumn", "UserId");
            this.userNameColumn = config.GetString("userNameColumn", "UserName");
            this.userEmailColumn = config.GetString("userEmailColumn", "Email");
            this.autoCreateTables = config.GetBoolean("autoCreateTables", true);
            this.autoInitialize = config.GetBoolean("autoInitialize", true);
            this.maxInvalidPasswordAttempts = config.GetInteger("maxInvalidPasswordAttempts", int.MaxValue);
            this.minRequiredNonalphanumericCharacters = config.GetInteger("minRequiredNonalphanumericCharacters");
            this.minRequiredPasswordLength = config.GetInteger("minRequiredPasswordLength", 1);
            this.requiresUniqueEmail = config.GetBoolean("requiresUniqueEmail");
            this.maxEmailLength = config.GetInteger("maxEmailLength", 254);
            this.maxUserNameLength = config.GetInteger("maxUserNameLength", 56);
            this.maxPasswordLength = config.GetInteger("maxPasswordLength", 128);
            this.emailStrengthRegularExpression = config.GetString(
                "emailStrengthRegularExpression", @"^[0-9a-zA-Z.+_-]+@[0-9a-zA-Z.+_-]+\.[a-zA-Z]{2,4}$");
            this.userNameRegularExpression = config.GetString("userNameRegularExpression", @"^[0-9a-zA-Z_-]+$");
            this.ApplicationName = config.GetString("applicationName", "/");
            this.allowEmailAsUserName = config.GetBoolean("allowEmailAsUserName", true);

            try
            {
                new Regex(this.emailStrengthRegularExpression);
            }
            catch (ArgumentException e)
            {
                throw new ProviderException("invalid value for emailStrengthRegularExpression", e);
            }

            try
            {
                new Regex(this.userNameRegularExpression);
            }
            catch (ArgumentException e)
            {
                throw new ProviderException("invalid value for userNameRegularExpression", e);
            }

            if (config.ContainsKey("passwordAttemptWindowInSeconds") && config.ContainsKey("passwordAttemptWindow"))
            {
                throw new ProviderException(
                    "passwordAttemptWindowInSeconds and passwordAttemptWindow cannot both be set");
            }

            if (config.ContainsKey("passwordAttemptWindowInSeconds"))
            {
                this.passwordAttemptWindowInSeconds = config.GetInteger("passwordAttemptWindowInSeconds", int.MaxValue);
            }
            else
            {
                var passwordAttemptWindowInMinutes = config.GetInteger("passwordAttemptWindow", -1);
                if (passwordAttemptWindowInMinutes < 0)
                {
                    this.passwordAttemptWindowInSeconds = int.MaxValue;
                }
                else
                {
                    this.passwordAttemptWindowInSeconds = passwordAttemptWindowInMinutes * 60;
                }
            }

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
            config.Remove("passwordAttemptWindow");
            config.Remove("passwordAttemptWindowInSeconds");
            config.Remove("maxEmailLength");
            config.Remove("maxUserNameLength");
            config.Remove("maxPasswordLength");
            config.Remove("emailStrengthRegularExpression");
            config.Remove("userNameRegularExpression");
            config.Remove("allowEmailAsUserName");

            var providerName = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
            if (connectionString != null)
            {
                providerName = connectionString.ProviderName;
            }

            this.sqlQueryBuilder = this.sqlQueryBuilderFactory(
                providerName, this.userTableName, this.userIdColumn, this.userNameColumn, this.userEmailColumn);

            base.Initialize(name, config);

            if (this.AutoInitialize)
            {
                this.InitializeDatabaseConnection();

                if (this.HasEmailColumnDefined)
                {
                    this.CreateUserEmailColumn();
                }
            }
        }

        public override string ResetPassword(string userName, string answer)
        {
            Condition.Requires(userName, "userName").IsNotNullOrEmpty();

            if (!string.IsNullOrWhiteSpace(answer))
            {
                throw new NotSupportedException("answer is not supported, pass an empty string or null");
            }

            var newPassword =
                Membership.GeneratePassword(
                    this.MinRequiredPasswordLength < PasswordSize ? PasswordSize : this.MinRequiredPasswordLength, 
                    this.MinRequiredNonAlphanumericCharacters);

            if (!this.HasLocalAccount(this.GetUserId(userName)))
            {
                this.CreateAccount(userName, newPassword, false);
                return newPassword;
            }

            return this.ResetPasswordAndUnlock(userName, newPassword) ? newPassword : null;
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            Assert(this.ValidatePasswordWithoutNotification(newPassword), MembershipCreateStatus.InvalidPassword);

            return base.ResetPasswordWithToken(token, newPassword);
        }

        public override bool UnlockUser(string userName)
        {
            Condition.Requires(userName, "userName").IsNotNullOrWhiteSpace();

            using (var db = this.ConnectToDatabase())
            {
                return db.Execute(this.sqlQueryBuilder.UnlockUser, userName) > 0;
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            Condition.Requires(user, "user").IsNotNull();

            Assert(this.ValidateUserName(user.UserName), MembershipCreateStatus.InvalidUserName);
            Assert(this.ValidateEmail(user.Email), MembershipCreateStatus.InvalidEmail);
            Assert(
                this.ValidateProviderUserKey(user.ProviderUserKey, false), MembershipCreateStatus.InvalidProviderUserKey);

            using (var db = this.ConnectToDatabase())
            {
                db.Execute(this.sqlQueryBuilder.UpdateUserMembership, user.ProviderUserKey, user.IsApproved);

                if (this.HasEmailColumnDefined)
                {
                    db.Execute(this.sqlQueryBuilder.UpdateUserEmail, user.ProviderUserKey, user.Email);
                }
            }
        }

        private static string AppendWildcardToSearchTerm(string emailToMatch)
        {
            return string.Concat("%", emailToMatch, "%");
        }

        private static void Assert(bool condition, MembershipCreateStatus status)
        {
            if (!condition)
            {
                throw new MembershipCreateUserException(status);
            }
        }

        private static DateTime GetDateTime(object value)
        {
            return value == null ? DateTime.MinValue : (DateTime)value;
        }

        private static int GetPagingStartRow(int pageIndex, int pageSize)
        {
            return pageIndex * pageSize;
        }

        private static int GetTotalRecords(IList<dynamic> rows)
        {
            return (int)rows[0][0];
        }

        private IDatabase ConnectToDatabase()
        {
            return this.databaseFactory(this.ConnectionStringName);
        }

        private MembershipUser CreateMembershipUser(dynamic row)
        {
            int userId = row[1];
            string name = row[2];
            bool isConfirmed = row[3] ?? true;
            DateTime lastPasswordFailureDate = GetDateTime(row[4]);
            int passwordFailuresSinceLastSuccess = row[5] ?? 0;
            DateTime creationDate = GetDateTime(row[6]);
            DateTime passwordChangedDate = GetDateTime(row[7]);
            string email = this.HasEmailColumnDefined ? row[8] : string.Empty;
            var isLockedOut = isConfirmed && passwordFailuresSinceLastSuccess > this.MaxInvalidPasswordAttempts
                              && lastPasswordFailureDate.Add(TimeSpan.FromSeconds(this.PasswordAttemptWindowInSeconds))
                              > DateTime.UtcNow;

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
                DateTime.MinValue);
        }

        private void CreateUserEmailColumn()
        {
            using (var db = this.ConnectToDatabase())
            {
                if (!db.ColumnExists(this.UserTableName, this.UserEmailColumn))
                {
                    if (this.RequiresUniqueEmail)
                    {
                        if (
                            !db.AddColumnToTable(
                                this.UserTableName, 
                                this.UserEmailColumn, 
                                SqlDbType.NVarChar, 
                                this.MaxEmailLength, 
                                false, 
                                true))
                        {
                            db.AddColumnToTable(
                                this.UserTableName, 
                                this.UserEmailColumn, 
                                SqlDbType.NVarChar, 
                                this.MaxEmailLength, 
                                true, 
                                false);
                        }
                    }
                    else
                    {
                        db.AddColumnToTable(
                            this.UserTableName, 
                            this.UserEmailColumn, 
                            SqlDbType.NVarChar, 
                            this.MaxEmailLength, 
                            false, 
                            false);
                    }
                }
            }
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
                System.Data.Entity.Database.SetInitializer<DbContext>(null);

                using (var context = new DbContext(this.ConnectionStringName))
                {
                    if (!context.Database.Exists())
                    {
                        context.Database.CreateIfNotExists();
                    }
                }

                this.webSecurityFacade.InitializeDatabaseConnection(
                    this.ConnectionStringName, 
                    this.userTableName, 
                    this.userIdColumn, 
                    this.userNameColumn, 
                    this.autoCreateTables);
            }
        }

        private bool ResetPasswordAndUnlock(string userName, string newPassword)
        {
            var hashedPassword = Crypto.HashPassword(newPassword);

            using (var db = this.ConnectToDatabase())
            {
                return db.Execute(this.sqlQueryBuilder.ResetPassword, userName, hashedPassword) > 0;
            }
        }

        private bool ValidateEmail(string email)
        {
            return this.ValidateEmail(email, this.RequiresUniqueEmail, this.MaxEmailLength);
        }

        private bool ValidateEmail(string email, bool requireUnique, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(email) && requireUnique)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            if (email.Length > maxLength)
            {
                return false;
            }

            if (this.EmailStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(email, this.EmailStrengthRegularExpression))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidatePassword(string password, string username)
        {
            if (!this.ValidatePasswordWithoutNotification(password))
            {
                return false;
            }

            var validatePasswordArgs = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(validatePasswordArgs);

            if (validatePasswordArgs.Cancel)
            {
                return false;
            }

            return true;
        }

        private bool ValidatePasswordWithoutNotification(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < this.MinRequiredPasswordLength
                || password.Length > this.MaxPasswordLength)
            {
                return false;
            }

            var count = password.Where((t, i) => !char.IsLetterOrDigit(password, i)).Count();

            if (count < this.MinRequiredNonAlphanumericCharacters)
            {
                return false;
            }

            if (this.PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateProviderUserKey(object providerUserKey, bool allowNull)
        {
            if (allowNull)
            {
                return providerUserKey == null || providerUserKey is int;
            }

            return providerUserKey is int && (int)providerUserKey > 0;
        }

        private bool ValidateUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            if (username.Contains("@") && this.AllowEmailAsUserName)
            {
                return this.ValidateEmail(username, true, this.MaxUserNameLength);
            }

            if (username.Length > this.MaxUserNameLength)
            {
                return false;
            }

            if (this.UserNameRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(username, this.UserNameRegularExpression))
                {
                    return false;
                }
            }

            return true;
        }
    }
}