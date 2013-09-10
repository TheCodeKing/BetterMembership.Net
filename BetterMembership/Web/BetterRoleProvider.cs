namespace BetterMembership.Web
{
    using System;
    using System.Collections.Specialized;

    using BetterMembership.Extensions;
    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using CuttingEdge.Conditions;

    using WebMatrix.WebData;

    public sealed class BetterRoleProvider : SimpleRoleProvider
    {
        private readonly WebSecurityFacade webSecurityFacade;

        private bool autoCreateTables;

        private string connectionStringName;

        private string userIdColumn;

        private string userNameColumn;

        private string userTableName;

        public BetterRoleProvider()
            : this(new WebSecurityFacade())
        {
        }

        internal BetterRoleProvider(WebSecurityFacade webSecurityFacade)
        {
            Condition.Requires(webSecurityFacade, "webSecurityFacade").IsNotNull();

            this.webSecurityFacade = webSecurityFacade;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();
            Condition.Requires(config, "config").IsNotNull();

            this.connectionStringName = config.GetString("connectionStringName", "DefaultConnection");
            this.userTableName = config.GetString("userTableName", "UserProfile");
            this.userIdColumn = config.GetString("userIdColumn", "UserId");
            this.userNameColumn = config.GetString("userNameColumn", "UserName");
            this.autoCreateTables = config.GetBoolean("autoCreateTables", true);
            var autoInitialize = config.GetBoolean("autoInitialize", true);

            config.Remove("userTableName");
            config.Remove("userIdColumn");
            config.Remove("userNameColumn");
            config.Remove("autoCreateTables");
            config.Remove("autoInitialize");

            base.Initialize(name, config);

            if (autoInitialize)
            {
                this.InitializeDatabaseConnection();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                return base.GetRolesForUser(username);
            }
            catch (InvalidOperationException)
            {
                return new string[0];
            }
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