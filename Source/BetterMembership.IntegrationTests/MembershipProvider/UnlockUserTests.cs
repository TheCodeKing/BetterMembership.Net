namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class UnlockUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedLockedOutUserWhenUnlockUserThenUserCanAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithConfirmedUser().WithPasswordLockout().Value;
            var lockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordAttemptWindowInSeconds());
            Assert.That(lockedOut, Is.True, "Failed to lockout account");

            // act
            var result = testClass.UnlockUser(testUser.UserName);

            // assert
            Assert.That(result, Is.True);
            lockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordAttemptWindowInSeconds());
            Assert.That(lockedOut, Is.False);
        }
    }
}