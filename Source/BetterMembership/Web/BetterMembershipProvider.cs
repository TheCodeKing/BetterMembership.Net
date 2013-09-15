namespace BetterMembership.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Data.Entity;
    using System.Linq;
    using System.Text.RegularExpressions;
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
        private static readonly Regex UserNameRegularExpression = new Regex("[^a-zA-Z0-9_-]", RegexOptions.Compiled);

        private readonly Func<string, IDatabase> databaseFactory;

        private readonly Func<string, string, string, string, string, ISqlQueryBuilder> sqlQueryBuilderFactory;

        private readonly IWebSecurityFacade webSecurityFacade;

        private bool autoCreateTables;

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

        private string userTableName;

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

        internal int PasswordAttemptWindowInSeconds
        {
            get
            {
                return this.passwordAttemptWindowInSeconds;
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Condition.Requires(username, "username").Evaluate(this.ValidateUserName(username));
            Condition.Requires(oldPassword, "oldPassword").IsNotNullOrWhiteSpace();
            Condition.Requires(newPassword, "newPassword").Evaluate(this.ValidatePassword(newPassword, username));

            return base.ChangePassword(username, oldPassword, newPassword);
        }

        public override bool ChangePasswordQuestionAndAnswer(
            string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationTotken)
        {
            Condition.Requires(userName, "userName").IsNotNullOrWhiteSpace();
            Condition.Requires(password, "password").IsNotNullOrWhiteSpace();

            Condition.Requires(userName, "userName").Evaluate(this.ValidateUserName(userName));
            Condition.Requires(password, "password").Evaluate(this.ValidatePassword(password, userName));

            return base.CreateAccount(userName, password, requireConfirmationTotken);
        }

        public override string CreateAccount(string userName, string password)
        {
            Condition.Requires(userName, "userName").Evaluate(this.ValidateUserName(userName));
            Condition.Requires(password, "password").Evaluate(this.ValidatePassword(password, userName));

            return base.CreateAccount(userName, password);
        }

        public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
        {
            Condition.Requires(userName, "userName").Evaluate(this.ValidateUserName(userName));

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
            Condition.Requires(userName, "userName").IsNotNullOrWhiteSpace();
            Condition.Requires(password, "password").IsNotNullOrWhiteSpace();

            Condition.Requires(userName, "userName").Evaluate(this.ValidateUserName(userName));
            Condition.Requires(password, "password").Evaluate(this.ValidatePassword(password, userName));

            if (this.RequiresUniqueEmail && (values == null || !values.ContainsKey(this.UserEmailColumn)))
            {
                throw new ArgumentException(string.Format("{0} is required", this.UserEmailColumn));
            }

            return base.CreateUserAndAccount(userName, password, requireConfirmation, values);
        }

        public override string CreateUserAndAccount(
            string userName, string password, IDictionary<string, object> values)
        {
            return base.CreateUserAndAccount(userName, password, false, values);
        }

        public override string CreateUserAndAccount(string userName, string password)
        {
            return base.CreateUserAndAccount(userName, password, false, null);
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation)
        {
            return base.CreateUserAndAccount(userName, password, requireConfirmation, null);
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
                var rows =
                    db.Query(this.sqlQueryBuilder.FindUsersByEmailQuery, startRow, pageSize, emailToMatch).ToList();
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
                var rows =
                    db.Query(this.sqlQueryBuilder.FindUsersByNameQuery, startRow, pageSize, usernameToMatch).ToList();
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
                var rows = db.Query(this.sqlQueryBuilder.GetAllUsersQuery, startRow, pageSize).ToList();
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
                var row = db.QuerySingle(this.sqlQueryBuilder.GetUserQuery, username);
                if (row != null)
                {
                    return this.CreateMembershipUser(row);
                }
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Condition.Requires(providerUserKey, "providerUserKey")
                     .Evaluate(this.ValidateProviderUserKey(providerUserKey, false));

            if (userIsOnline)
            {
                throw new NotSupportedException("value provided for userIsOnline is not supported");
            }

            var userId = (int)providerUserKey;

            using (var db = this.ConnectToDatabase())
            {
                var row = db.QuerySingle(this.sqlQueryBuilder.GetUserByIdQuery, userId);
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
            this.userEmailColumn = config.GetString("userEmailColumn");
            this.autoCreateTables = config.GetBoolean("autoCreateTables", true);
            var autoInitialize = config.GetBoolean("autoInitialize", true);
            this.maxInvalidPasswordAttempts = config.GetInteger("maxInvalidPasswordAttempts", int.MaxValue);
            this.minRequiredNonalphanumericCharacters = config.GetInteger("minRequiredNonalphanumericCharacters");
            this.minRequiredPasswordLength = config.GetInteger("minRequiredPasswordLength", 1);
            this.requiresUniqueEmail = config.GetBoolean("requiresUniqueEmail");
            this.maxEmailLength = config.GetInteger("maxEmailLength", 254);
            this.maxUserNameLength = config.GetInteger("maxUserNameLength", 56);
            this.maxPasswordLength = config.GetInteger("maxPasswordLength", 128);
            this.emailStrengthRegularExpression = config.GetString(
                "emailStrengthRegularExpression", @"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}");

            try
            {
                new Regex(this.emailStrengthRegularExpression);
            }
            catch (ArgumentException e)
            {
                throw new ProviderException("invalid value for emailStrengthRegularExpression", e);
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

            var providerName = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings[this.connectionStringName];
            if (connectionString != null)
            {
                providerName = connectionString.ProviderName;
            }

            this.sqlQueryBuilder = this.sqlQueryBuilderFactory(
                providerName, this.userTableName, this.userIdColumn, this.userNameColumn, this.userEmailColumn);

            base.Initialize(name, config);

            if (autoInitialize)
            {
                this.InitializeDatabaseConnection();
            }

            if (this.HasEmailColumnDefined)
            {
                this.CreateUserEmailColumn();
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            Condition.Requires(newPassword, "newPassword").IsNotNullOrWhiteSpace();
            Condition.Requires(newPassword, "newPassword")
                     .Evaluate(this.ValidatePasswordWithoutNotification(newPassword));

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
            Condition.Requires(user.UserName, "user.UserName").Evaluate(this.ValidateUserName(user.UserName));
            Condition.Requires(user.ProviderUserKey, "user.ProviderUserKey")
                     .Evaluate(this.ValidateProviderUserKey(user.ProviderUserKey, false));
            Condition.Requires(user.Email, "user.Email").Evaluate(this.ValidateEmail(user.Email));

            using (var db = this.ConnectToDatabase())
            {
                db.Execute(this.sqlQueryBuilder.UpdateUserMembership, user.ProviderUserKey, user.IsApproved);

                if (this.HasEmailColumnDefined)
                {
                    db.Execute(this.sqlQueryBuilder.UpdateUserProfile, user.ProviderUserKey, user.Email);
                }
            }
        }

        private static string AppendWildcardToSearchTerm(string emailToMatch)
        {
            return string.Concat("%", emailToMatch, "%");
        }

        private static bool CheckEmailColumnExists(IDatabase db, string tableName, string columnName)
        {
            var query =
                db.QuerySingle(
                    @"SELECT * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @0 and COLUMN_NAME = @1", 
                    tableName, 
                    columnName);
            return query != null;
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
                if (!CheckEmailColumnExists(db, this.UserTableName, this.UserEmailColumn))
                {
                    if (this.RequiresUniqueEmail)
                    {
                        try
                        {
                            db.Execute(
                                @"ALTER TABLE [" + this.UserTableName + "] ADD [" + this.UserEmailColumn + "] nvarchar("
                                + this.MaxEmailLength + ") NOT NULL UNIQUE");
                        }
                        catch (Exception)
                        {
                            try
                            {
                                db.Execute(
                                    @"ALTER TABLE [" + this.UserTableName + "] ADD [" + this.UserEmailColumn
                                    + "] nvarchar(" + this.MaxEmailLength + ") NULL");
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            db.Execute(
                                @"ALTER TABLE [" + this.UserTableName + "] ADD [" + this.UserEmailColumn + "] nvarchar("
                                + this.MaxEmailLength + ") NULL");
                        }
                        catch
                        {
                        }
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
                using (var context = new DbContext(this.connectionStringName))
                {
                    if (!context.Database.Exists())
                    {
                        context.Database.CreateIfNotExists();
                    }
                }

                this.webSecurityFacade.InitializeDatabaseConnection(
                    this.connectionStringName, 
                    this.userTableName, 
                    this.userIdColumn, 
                    this.userNameColumn, 
                    this.autoCreateTables);
            }
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) && this.RequiresUniqueEmail)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            if (email.Length > this.MaxEmailLength)
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
            return !string.IsNullOrWhiteSpace(username) && username.Length <= this.MaxUserNameLength
                   && !UserNameRegularExpression.IsMatch(username);
        }
    }
}