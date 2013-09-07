namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class UnlockUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenConfirmedLockedOutUserWhenUnlockUserThenUserCanAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithConfirmedUser().WithPasswordLockout().Value;
            var lockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());
            Assert.That(lockedOut, Is.True);

            // act
            var result = testClass.UnlockUser(testUser.UserName);

            // assert
            Assert.That(result, Is.True);
            lockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());
            Assert.That(lockedOut, Is.False);
        }
    }
}