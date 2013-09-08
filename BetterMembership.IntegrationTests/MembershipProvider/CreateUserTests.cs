namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class CreateUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        public void GivenUnregisteredUserWhenCreateUserThenMembershipUserReturned(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            string email = testUser.Email;
            if (!testClass.AsBetter().HasEmailColumnDefined)
            {
                email = null;
            }

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(
                testUser.UserName, testUser.Password, email, null, null, true, null, out status);

            // assert
            var result = testClass.GetUser(testUser.UserName, false);
            Assert.That(user, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(testUser.UserName));
            if (testClass.AsBetter().HasEmailColumnDefined)
            {
                Assert.That(result.Email, Is.EqualTo(testUser.Email));
            }
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
            var user = testClass.CreateUser(testUser2.UserName, testUser2.Password, testUser2.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.DuplicateEmail));
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserWithDuplicateUserNameThenStatusDuplicateUserName(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);

            var testUser1 = testClass.WithConfirmedUser().Value;
            var testUser2 = testClass.WithUnregisteredUser(testUser1.UserName, "test@test.com").Value;
            if (!testClass.HasEmailColumnDefined())
            {
                testUser1.Email = null;
                testUser2.Email = null;
            }

            // act
            MembershipCreateStatus status;
            var user = testClass.CreateUser(testUser2.UserName, testUser2.Password, testUser2.Email, null, null, true, null, out status);

            // assert
            Assert.That(user, Is.Null);
            Assert.That(status, Is.EqualTo(MembershipCreateStatus.DuplicateUserName));
        }
    }
}