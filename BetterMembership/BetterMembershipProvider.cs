namespace BetterMembership
{
    using System;
    using System.Collections.Specialized;
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

        private readonly WebSecurityFacade webSecurityFacade;

        private bool autoCreateTables;

        private string connectionStringName;

        private string userEmailColumn;

        private string userIdColumn;

        private string userNameColumn;

        private string userTableName;

        public BetterMembershipProvider()
            : this(new WebSecurityFacade(), s => new DatabaseProxy(Database.Open(s)))
        {
        }

        internal BetterMembershipProvider(WebSecurityFacade webSecurityFacade, Func<string, IDatabase> databaseFactory)
        {
            Condition.Requires(webSecurityFacade, "webSecurityFacade").IsNotNull();
            Condition.Requires(databaseFactory, "databaseFactory").IsNotNull();

            this.webSecurityFacade = webSecurityFacade;
            this.databaseFactory = databaseFactory;
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return base.RequiresUniqueEmail;
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
            return base.CreateUser(
                username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public override MembershipUserCollection FindUsersByEmail(
            string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return base.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return base.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return base.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            return base.GetNumberOfUsersOnline();
        }

        public override string GetPassword(string username, string answer)
        {
            return base.GetPassword(username, answer);
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            return base.GetPasswordChangedDate(userName);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return base.GetUser(providerUserKey, userIsOnline);
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            return base.GetUserIdFromPasswordResetToken(token);
        }

        public override string GetUserNameByEmail(string email)
        {
            return base.GetUserNameByEmail(email);
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

            config.Remove("userTableName");
            config.Remove("userIdColumn");
            config.Remove("userNameColumn");
            config.Remove("userEmailColumn");
            config.Remove("autoCreateTables");
            config.Remove("autoInitialize");

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
            base.UpdateUser(user);
        }

        private IDatabase ConnectToDatabase()
        {
            return this.databaseFactory(this.connectionStringName);
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