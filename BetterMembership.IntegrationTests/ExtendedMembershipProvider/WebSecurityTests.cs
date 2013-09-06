namespace BetterMembership.IntegrationTests.ExtendedMembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class WebSecurityTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenMultipleProvidersWhenNonDefaultProviderUsedForCreateUserAndAccountThenUserAccountIsCreated(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;

            // act
            var result = testClass.CreateUserAndAccount(testUser.UserName, testUser.Password, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;

            // act
            var webSecurityResult = testClass.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordLockoutTimeoutInSeconds());

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserAndAccountThenUserAccountIsCreated(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;

            // act
            var id = testClass.CreateUserAndAccount(testUser.UserName, testUser.Password, true, null);

            // assert
            Assert.That(id, Is.Not.Null);
        }
    }
}