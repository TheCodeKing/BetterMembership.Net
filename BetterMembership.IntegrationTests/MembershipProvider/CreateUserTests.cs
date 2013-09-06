namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Threading;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class CreateUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
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
            var user = testClass.CreateUser(testUser.UserName, testUser.Password, email, null, null, true, null, out status);

            // assert
            var result = testClass.GetUser(testUser.UserName, false);
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            if (testClass.AsBetter().HasEmailColumnDefined)
            {
                Assert.That(user.Email, Is.EqualTo(testUser.Email));
            }

        }
    }
}