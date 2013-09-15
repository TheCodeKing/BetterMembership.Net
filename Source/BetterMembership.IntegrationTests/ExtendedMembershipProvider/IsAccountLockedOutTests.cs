namespace BetterMembership.IntegrationTests.ExtendedMembershipProvider
{
    using System.Threading;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class IsAccountLockedOutTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            (string providerName)
        {
            // arrange
            const int PasswordAttemptWindowInSeconds = 1;
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            Thread.Sleep((PasswordAttemptWindowInSeconds * 1000) + 500);
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, PasswordAttemptWindowInSeconds);

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            const int PasswordAttemptWindowInSeconds = 1;
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, PasswordAttemptWindowInSeconds);

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenLessThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            const int PasswordAttemptWindowInSeconds = 1;
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, PasswordAttemptWindowInSeconds);

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }
    }
}