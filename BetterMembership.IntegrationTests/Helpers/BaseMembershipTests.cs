namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Security;

    using NUnit.Framework;

    using WebMatrix.WebData;

    public abstract class BaseMembershipTests
    {
        protected const string SqlClientProviderNameWithEmail = "SqlClientProviderWithEmailColumn";

        protected const string SqlClientProviderWithUniqueEmail = "SqlClientProviderWithUniqueEmail";

        protected const string SqlClientProviderNameWithoutEmail = "SqlClientProviderWithoutEmailColumn";

        protected const string SqlClientCeProviderNameWithEmail = "SqlClientCeProviderWithEmailColumn";

        protected const string SqlClientCeProviderWithUniqueEmail = "SqlClientCeProviderWithUniqueEmail";

        protected const string SqlClientCeProviderNameWithoutEmail = "SqlClientCeProviderWithoutEmailColumn";

        [SetUp]
        public virtual void SetUp()
        {
            HttpContext.Current.WithHttpContext().WithCleanDatabase();
        }

        protected MembershipProvider WithProvider(string provider)
        {
            return Membership.Providers[provider];
        }

        protected ExtendedMembershipProvider WithExtendedProvider(string providerName)
        {
            return this.WithProvider(providerName) as ExtendedMembershipProvider;
        }
    }
}