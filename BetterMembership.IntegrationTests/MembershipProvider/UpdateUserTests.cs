namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class UpdateUserTests : BaseMembershipTests
    {
        private const string NewEmail = "newemail@test.com";

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenLoginThenUserCanAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            var webSecurityResult = testClass.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.True);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenConfirmedUserWhenUpdateUserThenUserIsUpdated(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;
            var user = testClass.GetUser(testUser.UserName, false);
            user.IsApproved = false;
            if (testClass.HasEmailColumnDefined())
            {
                user.Email = NewEmail;
            }

            // act
            testClass.UpdateUser(user);

            // assert 
            var reloadUser = testClass.GetUser(testUser.UserName, false);
            Assert.That(reloadUser, Is.Not.Null);
            Assert.That(reloadUser.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(reloadUser.IsApproved, Is.False);
            if (testClass.HasEmailColumnDefined())
            {
                Assert.That(reloadUser.Email, Is.EqualTo(NewEmail));
            }
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenUnConfirmedUserWhenUpdateUserThenUserIsUpdated(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;
            var user = testClass.GetUser(testUser.UserName, false);
            user.IsApproved = true;
            if (testClass.HasEmailColumnDefined())
            {
                user.Email = NewEmail;
            }

            // act
            testClass.UpdateUser(user);

            // assert 
            var reloadUser = testClass.GetUser(testUser.UserName, false);
            Assert.That(reloadUser, Is.Not.Null);
            Assert.That(reloadUser.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(reloadUser.IsApproved, Is.True);
            if (testClass.HasEmailColumnDefined())
            {
                Assert.That(reloadUser.Email, Is.EqualTo(NewEmail));
            }
        }
    }
}