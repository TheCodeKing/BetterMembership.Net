namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class UpdateUserTests : BaseMembershipTests
    {
        private const string NewEmail = "newemail@test.com";

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenUpdateUserThenUserIsUpdated(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.IsApproved = false;
            if (testClass.HasEmailColumnDefined())
            {
                testUser.Email = NewEmail;
            }

            // act
            testClass.UpdateUser(testUser);

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

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenUpdateUserThenUserIsUpdated(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().AsMembershipUser();
            testUser.IsApproved = true;
            if (testClass.HasEmailColumnDefined())
            {
                testUser.Email = NewEmail;
            }

            // act
            testClass.UpdateUser(testUser);

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

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWithInvalidEmailWhenUpdateUserThenMembershipCreateUserException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = "email";

            // act // assert
            Assert.Throws<MembershipCreateUserException>(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenConfirmedUserWithEmailTooLongWhenUpdateUserThenMembershipCreateUserException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = "@x.com".PadLeft(testClass.AsBetter().MaxEmailLength + 1);

            // act // assert
            Assert.Throws<MembershipCreateUserException>(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        public void GivenConfirmedUserWithEmailNullWhenUpdateUserThenArgumentException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = null;

            // act // assert
            Assert.DoesNotThrow(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenConfirmedUserWithUniqueEmailNullWhenUpdateUserThenMembershipCreateUserException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = null;

            // act // assert
            Assert.Throws<MembershipCreateUserException>(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        public void GivenConfirmedUserWithEmailEmptyWhenUpdateUserThenArgumentException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = string.Empty;

            // act // assert
            Assert.DoesNotThrow(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenConfirmedUserWithUniqueEmailEmptyWhenUpdateUserThenMembershipCreateUserException(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().AsMembershipUser();
            testUser.Email = string.Empty;

            // act // assert
            Assert.Throws<MembershipCreateUserException>(() => testClass.UpdateUser(testUser));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWithEmailMaxLengthWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser =
                testClass.WithConfirmedUser().WithEmailLength(testClass.AsBetter().MaxEmailLength).AsMembershipUser();

            // act
            Assert.DoesNotThrow(() => testClass.UpdateUser(testUser));
        }
    }
}