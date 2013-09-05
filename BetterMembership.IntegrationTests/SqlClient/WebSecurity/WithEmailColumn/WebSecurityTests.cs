namespace BetterMembership.IntegrationTests.SqlClient.WebSecurity.WithEmailColumn
{
    using System.Threading;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class WebSecurityTests
    {
        private ExtendedMembershipProvider provider;

        [SetUp]
        public void SetUp()
        {
            HttpContext.Current.WithHttpContext().WithCleanDatabase();

            this.provider = Membership.Providers["defaultProviderWithEmailColumn"] as ExtendedMembershipProvider;
        }

        [Test]
        public void GivenConfirmedUserWhenLoginThenUserCanAuthenticate()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();

            // act
            var webSecurityResult = this.provider.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.True);
        }

        [Test]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            ()
        {
            // arrange
            var testUser =
                this.provider.WithConfirmedUser().WithInvalidPasswordAttempts(Constants.ConfiguredAllowedPasswordAttempts + 1);

            // act
            Thread.Sleep(1000 * Constants.ConfiguredPasswordLockoutTimeoutInSeconds);
            var webSecurityIsLockedOut = WebSecurity.IsAccountLockedOut(
                testUser.UserName, Constants.ConfiguredAllowedPasswordAttempts, Constants.ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [Test]
        public void GivenMultipleProvidersWhenNonDefaultProviderUsedForCreateUserAndAccountThenUserAccountIsCreated()
        {
            // arrange
            var testUser = this.provider.WithUnregisteredUser();

            // act
            var result = this.provider.CreateUserAndAccount(testUser.UserName, testUser.Password, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate()
        {
            // arrange
            var testUser = this.provider.WithUnconfirmedUser();

            // act
            var webSecurityResult = this.provider.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut()
        {
            // arrange
            var testUser =
                this.provider.WithUnconfirmedUser().WithInvalidPasswordAttempts(Constants.ConfiguredAllowedPasswordAttempts + 1);

            // act
            var webSecurityIsLockedOut = this.provider.IsAccountLockedOut(
                testUser.UserName, Constants.ConfiguredAllowedPasswordAttempts, Constants.ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [Test]
        public void GivenUnregisteredUserWhenCreateUserAndAccountThenUserAccountIsCreated()
        {
            // arrange
            var testUser = this.provider.WithUnregisteredUser();

            // act
            var id = this.provider.CreateUserAndAccount(testUser.UserName, testUser.Password, true, null);

            // assert
            Assert.That(id, Is.Not.Null);
        }
    }
}