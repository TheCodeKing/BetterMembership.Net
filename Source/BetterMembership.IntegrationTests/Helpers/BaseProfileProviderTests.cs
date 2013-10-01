namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Profile;
    using System.Web.Security;

    using NUnit.Framework;

    public abstract class BaseProfileProviderTests
    {
        protected const string SqlClientCeProfileProviderWithEmail = "SqlClientCeProfileProviderWithEmail";

        protected const string SqlClientCeProfileProviderWithoutEmail = "SqlClientCeProfileProviderWithoutEmail";

        protected const string SqlClientCeProfileProviderWithUniqueEmail = "SqlClientCeProfileProviderWithUniqueEmail";

        protected const string SqlClientProfileProviderWithEmail = "SqlClientProfileProviderWithEmail";

        protected const string SqlClientProfileProviderWithUniqueEmail = "SqlClientProfileProviderWithUniqueEmail";

        protected const string SqlClientProfileProviderWithoutEmail = "SqlClientProfileProviderWithoutEmail";

        [SetUp]
        public virtual void SetUp()
        {
            HttpContext.Current.WithHttpContext().WithCleanDatabase();
        }

        protected ProfileProvider WithProvider(string provider = null)
        {
            return provider == null ? ProfileManager.Provider : ProfileManager.Providers[provider];
        }

        protected MembershipProvider WithMembershipProvider(string provider = null)
        {
            return provider == null ? Membership.Provider : Membership.Providers[provider];
        }
    }
}