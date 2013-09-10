namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Security;

    using NUnit.Framework;

    using WebMatrix.WebData;

    public abstract class BaseMembershipTests
    {
        protected const string SqlClientCeProviderNameWithEmail = "SqlClientCeProviderWithEmailColumn";

        protected const string SqlClientCeProviderNameWithoutEmail = "SqlClientCeProviderWithoutEmailColumn";

        protected const string SqlClientCeProviderWithUniqueEmail = "SqlClientCeProviderWithUniqueEmail";

        protected const string SqlClientProviderNameWithEmail = "SqlClientProviderWithEmailColumn";

        protected const string SqlClientProviderNameWithoutEmail = "SqlClientProviderWithoutEmailColumn";

        protected const string SqlClientProviderWithUniqueEmail = "SqlClientProviderWithUniqueEmail";

        [SetUp]
        public virtual void SetUp()
        {
            HttpContext.Current.WithHttpContext().WithCleanDatabase();
        }

        protected ExtendedMembershipProvider WithExtendedProvider(string providerName)
        {
            return this.WithProvider(providerName) as ExtendedMembershipProvider;
        }

        protected MembershipProvider WithProvider(string provider)
        {
            return Membership.Providers[provider];
        }
    }
}