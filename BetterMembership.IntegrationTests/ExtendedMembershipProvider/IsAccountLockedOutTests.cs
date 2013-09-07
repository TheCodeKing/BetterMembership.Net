namespace BetterMembership.IntegrationTests.ExtendedMembershipProvider
{
    using System.Threading;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class IsAccountLockedOutTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            (string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser().WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1).Value;

            // act
            Thread.Sleep((1000 * testClass.PasswordLockoutTimeoutInSeconds()) + 500);
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser().WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1).Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }


        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenLessThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser().WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1).Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }
    }
}