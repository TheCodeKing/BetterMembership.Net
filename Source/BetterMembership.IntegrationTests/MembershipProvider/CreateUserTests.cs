namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Collections.Specialized;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class CreateUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserThenMembershipUserReturned(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            var result = testClass.GetUser(testUser.UserName, false);
            Assert.That(user, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(testUser.UserName));
            if (testClass.AsBetter().HasEmailColumnDefined)
            {
                Assert.That(result.Email, Is.EqualTo(testUser.Email));
            }
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserWithDuplicateUserNameThenStatusDuplicateUserName(
            string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);

            var testUser1 = testClass.WithConfirmedUser().Value;
            var testUser2 = testClass.WithUnregisteredUser(testUser1.UserName, "different@test.com").Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser2.UserName, testUser2.Password, testUser2.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.DuplicateUserName));
        }

        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWhenCreateUserWithDuplicateEmailThenMembershipUserReturned(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);

            var testUser1 = testClass.WithConfirmedUser().Value;
            var testUser2 = testClass.WithUnregisteredUser(email: testUser1.Email).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser2.UserName, testUser2.Password, testUser2.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.DuplicateEmail));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserWithUnApprovedFlagThenUserUnconfirmed(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser1 = testClass.WithUnregisteredUser().Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser1.UserName, testUser1.Password, testUser1.Email, null, null, false, null, out status);

            // assert
            Assert.That(user.IsApproved, Is.False);
            Assert.That(testClass.AsBetter().IsConfirmed(testUser1.UserName), Is.False);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserWithApprovedFlagThenUserConfirmed(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser1 = testClass.WithUnregisteredUser().Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser1.UserName, testUser1.Password, testUser1.Email, null, null, true, null, out status);

            // assert
            Assert.That(user.IsApproved, Is.True);
            Assert.That(testClass.AsBetter().IsConfirmed(testUser1.UserName), Is.True);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithNotMinNonAlphaNumericPasswordWhenCreateUserThenInvalidPasswordStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                         .WithNonAlphaNumericCharsInPassword(testClass.MinRequiredNonAlphanumericCharacters - 1)
                         .Value;

            // act
            MembershipCreateStatus status;
            testClass.CreateUser(testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidPassword));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithMoreThanMinNonAlphaNumbericPasswordWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                .WithNonAlphaNumericCharsInPassword(testClass.MinRequiredNonAlphanumericCharacters + 1).Value;

            // act
            MembershipCreateStatus status;
            testClass.CreateUser(testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithExactMinNonAlphaNumbericPasswordWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                .WithNonAlphaNumericCharsInPassword(testClass.AsBetter().MinRequiredNonAlphanumericCharacters).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithTooLongPasswordWhenCreateUserThenInvalidPasswordStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                .WithPasswordLength(testClass.AsBetter().MaxPasswordLength + 1).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidPassword));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithExactMinPasswordWhenCreateUserThenSuccesStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                .WithPasswordLength(testClass.AsBetter().MaxPasswordLength).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithTooFewCharsWhenCreateUserThenInvalidPasswordStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithPasswordLength(testClass.MinRequiredPasswordLength - 1).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidPassword));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithExactPasswordLengthWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithPasswordLength(testClass.MinRequiredPasswordLength).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithInvalidUserNameWhenCreateUserThenInvalidUserNameStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.UserName = "<script>";

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidUserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithUserNameTooLongWhenCreateUserThenInvalidUserNameStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithUserNameLength(testClass.AsBetter().MaxUserNameLength + 1).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidUserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithUserNameNullWhenCreateUserThenInvalidUserNameStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.UserName = null;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidUserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithUserNameEmptyWhenCreateUserThenInvalidUserNameStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.UserName = string.Empty;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidUserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithUserNameMaxLengthWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithUserNameLength(testClass.AsBetter().MaxUserNameLength).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWithInvalidEmailWhenCreateUserThenInvalidEmailStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.Email = "email";

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidEmail));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWithEmailTooLongWhenCreateUserThenInvalidEmailStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithEmailLength(testClass.AsBetter().MaxEmailLength + 1).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidEmail));
        }

        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientProviderWithEmail)]
        public void GivenUnregisteredUserWithEmailNullWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.Email = null;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWithUniqueEmailNullWhenCreateUserThenInvalidEmailStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.Email = null;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidEmail));
        }

        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWithUniqueEmailEmptyWhenCreateUserThenInvalidEmailStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.Email = string.Empty;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.InvalidEmail));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        public void GivenUnregisteredUserWithEmailEmptyWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            testUser.Email = string.Empty;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWithEmailMaxLengthWhenCreateUserThenSuccessStatus(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser()
                                    .WithEmailLength(testClass.AsBetter().MaxEmailLength).Value;

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, testUser.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.Success));
        }
    }
}