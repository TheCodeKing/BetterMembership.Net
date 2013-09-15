namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Security;

    using NUnit.Framework;

    public abstract class BaseRoleMembershipTests
    {
        protected const string SqlClientCeMembershipProviderName = "SqlClientCeProviderWithoutEmailColumn";

        protected const string SqlClientCeRoleProviderName = "SqlClientCeRoleProviderName";

        protected const string SqlClientMembershipProviderName = "SqlClientProviderWithoutEmailColumn";

        protected const string SqlClientRoleProviderName = "SqlClientRoleProviderName";

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