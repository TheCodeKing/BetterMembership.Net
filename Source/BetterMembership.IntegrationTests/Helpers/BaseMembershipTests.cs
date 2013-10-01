namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web;
    using System.Web.Security;

    using NUnit.Framework;

    using WebMatrix.WebData;

    public abstract class BaseMembershipTests
    {
        public const string SqlClientCeProviderWithEmail = "SqlClientCeProviderWithEmail";

        public const string SqlClientCeProviderWithoutEmail = "SqlClientCeProviderWithoutEmail";

        public const string SqlClientCeProviderWithUniqueEmail = "SqlClientCeProviderWithUniqueEmail";

        public const string SqlClientProviderWithEmail = "SqlClientProviderWithEmail";

        public const string SqlClientProviderWithoutEmail = "SqlClientProviderWithoutEmail";

        public const string SqlClientProviderWithUniqueEmail = "SqlClientProviderWithUniqueEmail";

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