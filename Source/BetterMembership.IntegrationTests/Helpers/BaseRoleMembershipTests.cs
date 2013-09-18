namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Security;

    using NUnit.Framework;

    public abstract class BaseRoleMembershipTests
    {
        protected const string SqlClientCeProviderWithEmail = "SqlClientCeProviderWithEmail";

        protected const string SqlClientCeProviderWithoutEmail = "SqlClientCeProviderWithoutEmail";

        protected const string SqlClientCeProviderWithUniqueEmail = "SqlClientCeProviderWithUniqueEmail";

        protected const string SqlClientProviderWithEmail = "SqlClientProviderWithEmail";

        protected const string SqlClientProviderWithoutEmail = "SqlClientProviderWithoutEmail";

        protected const string SqlClientProviderWithUniqueEmail = "SqlClientProviderWithUniqueEmail";

        protected const string SqlClientCeRoleProviderWithEmail = "SqlClientCeRoleProviderWithEmail";

        protected const string SqlClientCeRoleProviderWithoutEmail = "SqlClientCeRoleProviderWithoutEmail";

        protected const string SqlClientCeRoleProviderWithUniqueEmail = "SqlClientCeRoleProviderWithUniqueEmail";

        protected const string SqlClientRoleProviderWithEmail = "SqlClientRoleProviderWithEmail";

        protected const string SqlClientRoleProviderWithoutEmail = "SqlClientRoleProviderWithoutEmail";

        protected const string SqlClientRoleProviderWithUniqueEmail = "SqlClientRoleProviderWithUniqueEmail";

        [SetUp]
        public virtual void SetUp()
        {
            HttpContext.Current.WithHttpContext().WithCleanDatabase();
        }

        protected RoleProvider WithProvider(string provider)
        {
            return Roles.Providers[provider];
        }

        protected MembershipProvider WithMembershipProvider(string provider)
        {
            return Membership.Providers[provider];
        }
    }
}