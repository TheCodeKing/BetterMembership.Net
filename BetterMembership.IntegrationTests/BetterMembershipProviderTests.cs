namespace BetterMembership.IntegrationTests
{
    using System;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class BetterMembershipProviderTests
    {
        [SetUp]
        public void SetUp()
        {
            Helper.ClearDownDatabaseTables();

            HttpContext.Current.SetupCurrentHttpContext();
        }

        [Test]
        public void GivenMultipleProvidersWhenNonDefaultProviderCreateUserAndAccountThenReturnSuccess()
        {
            //// arrange
            var userName = Guid.NewGuid().ToString("N");
            const string Password = "Password1!";
            var provider = (ExtendedMembershipProvider)Membership.Providers["nonDefaultProvider"];

            // act
            var result = provider.CreateUserAndAccount(userName, Password, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenValidLoginCredentialsWhenLoginThenReturnSuccess()
        {
            //// arrange
            var userName = Guid.NewGuid().ToString("N");
            const string Password = "Password1!";
            WebSecurity.CreateUserAndAccount(userName, Password);

            // act
            var result = WebSecurity.Login(userName, Password);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenValidUserDetailsWhenCreateUserAndAccountThenSuccess()
        {
            //// arrange
            var userName = Guid.NewGuid().ToString("N");
            const string Password = "Password1!";

            // act
            var id = WebSecurity.CreateUserAndAccount(userName, Password, null, true);

            // assert
            Assert.That(id, Is.Not.Null);
        }
    }
}