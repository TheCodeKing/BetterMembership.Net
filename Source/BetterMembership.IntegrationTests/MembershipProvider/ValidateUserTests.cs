namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ValidateUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenLoginThenUserCanAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            var result = testClass.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;

            // act
            var result = testClass.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(result, Is.False);
        }
    }
}