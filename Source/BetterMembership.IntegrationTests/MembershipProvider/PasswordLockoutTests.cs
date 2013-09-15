namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System;
    using System.Threading;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class PasswordLockoutTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenExactlyMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser().WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts).Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            (string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            Thread.Sleep((testClass.PasswordAttemptWindowInSeconds() * 1000) + 500);
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountLockedOutSuccess(
            string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.True);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(user.IsLockedOut, Is.False);
        }
    }
}