namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class MembershipProviderTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenPasswordChangedThenChangedSuccessfully(string providerName)
        {
            // arrange
            const string NewPassword = "newPassword!";
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            var result = testClass.ChangePassword(testUser.UserName, testUser.Password, NewPassword);

            // assert
            Assert.That(result, Is.True);
            Assert.That(testClass.ValidateUser(testUser.UserName, NewPassword), Is.True);
        }
    }
}