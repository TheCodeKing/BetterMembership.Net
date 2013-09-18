namespace BetterMembership.IntegrationTests.ExtendedMembershipProvider
{
    using System.Threading;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class IsAccountLockedOutTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
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

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
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

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
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