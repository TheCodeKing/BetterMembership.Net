namespace BetterMembership.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Web.Security;

    using BetterMembership.Extensions;
    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using CuttingEdge.Conditions;

    using WebMatrix.WebData;

    public sealed class BetterRoleProvider : SimpleRoleProvider
    {
        private readonly MembershipProviderCollection membershipProviders;

        private readonly WebSecurityFacade webSecurityFacade;

        private BetterMembershipProvider membershipProvider;

        public BetterRoleProvider()
            : this(new WebSecurityFacade(), Membership.Providers)
        {
        }

        internal BetterRoleProvider(
            WebSecurityFacade webSecurityFacade, MembershipProviderCollection membershipProviders)
        {
            Condition.Requires(webSecurityFacade, "webSecurityFacade").IsNotNull();
            Condition.Requires(membershipProviders, "membershipProviders").IsNotNull();

            this.webSecurityFacade = webSecurityFacade;
            this.membershipProviders = membershipProviders;
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

        public override void Initialize(string name, NameValueCollection config)
        {
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();
            Condition.Requires(config, "config").IsNotNull();

            var membershipProviderName = config.GetString("membershipProviderName");

            if (!string.IsNullOrWhiteSpace(membershipProviderName))
            {
                this.membershipProvider = this.membershipProviders[membershipProviderName] as BetterMembershipProvider;
            }

            if (this.membershipProvider == null)
            {
                throw new ProviderException("membershipProviderName is required");
            }

            config.Remove("membershipProviderName");

            base.Initialize(name, config);

            if (this.membershipProvider.AutoInitialize)
            {
                this.InitializeDatabaseConnection();
            }
        }

        private void InitializeDatabaseConnection()
        {
            using (new DefaultProviderSwitcher(this))
            {
                this.webSecurityFacade.InitializeDatabaseConnection(
                    this.membershipProvider.ConnectionStringName, 
                    this.membershipProvider.UserTableName, 
                    this.membershipProvider.UserIdColumn, 
                    this.membershipProvider.UserNameColumn, 
                    this.membershipProvider.AutoCreateTables);
            }
        }
    }
}