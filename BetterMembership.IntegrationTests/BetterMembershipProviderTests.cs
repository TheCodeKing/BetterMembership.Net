namespace BetterMembership.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public void GivenAValidUserWhenGetUserThenMembershipUserSuccess()
        {
            // arrange
            var userName = Guid.NewGuid().ToString("N");
            const string Password = "Password1!";
            WebSecurity.CreateUserAndAccount(userName, Password, null, true);

            // act
            var user = Membership.GetUser(userName, false);

            // assert
            Assert.That(user, Is.Not.Null);
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
            // arrange
            var userName = Guid.NewGuid().ToString("N");
            const string Password = "Password1!";

            // act
            var id = WebSecurity.CreateUserAndAccount(userName, Password, null, true);

            // assert
            Assert.That(id, Is.Not.Null);
        }

        [Test]
        public void GivenValidUsersWhenFindUsersbyEmailThenResultsSuccess()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 1;
            string preFix1;
            string preFix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = Helper.WithUsers(UserGroup1Count, out preFix1);
            var users2 = Helper.WithUsers(UserGroup2Count, out preFix2);

            // act
            int totalRecords1;
            int totalRecords2;
            int totalRecords3;
            var results1 = Membership.FindUsersByEmail(preFix1, PageIndex, PageSize, out totalRecords1);
            var results2 = Membership.FindUsersByEmail(preFix2, PageIndex, PageSize, out totalRecords2);
            var results3 = Membership.FindUsersByEmail("missing", PageIndex, PageSize, out totalRecords3);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results2, Is.Not.Null);
            Assert.That(results3, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(PageSize));
            Assert.That(results2.Count, Is.EqualTo(1));
            Assert.That(results3.Count, Is.EqualTo(0));
            Assert.That(totalRecords1, Is.EqualTo(UserGroup1Count));
            Assert.That(totalRecords2, Is.EqualTo(UserGroup2Count));
            Assert.That(totalRecords3, Is.EqualTo(0));
            Assert.That(results1.ToArray()[0].UserName, Is.EqualTo(users1[2].UserName));
            Assert.That(results2.ToArray()[0].UserName, Is.EqualTo(users2[2].UserName));
        }

        [Test]
        public void GivenValidUsersWhenFindUsersbyUserNameThenResultsSuccess()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 1;
            string preFix1;
            string preFix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = Helper.WithUsers(UserGroup1Count, out preFix1);
            var users2 = Helper.WithUsers(UserGroup2Count, out preFix2);

            // act
            int totalRecords1;
            int totalRecords2;
            int totalRecords3;
            var results1 = Membership.FindUsersByName(preFix1, PageIndex, PageSize, out totalRecords1);
            var results2 = Membership.FindUsersByName(preFix2, PageIndex, PageSize, out totalRecords2);
            var results3 = Membership.FindUsersByName("missing", PageIndex, PageSize, out totalRecords3);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results2, Is.Not.Null);
            Assert.That(results3, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(PageSize));
            Assert.That(results2.Count, Is.EqualTo(1));
            Assert.That(results3.Count, Is.EqualTo(0));
            Assert.That(totalRecords1, Is.EqualTo(UserGroup1Count));
            Assert.That(totalRecords2, Is.EqualTo(UserGroup2Count));
            Assert.That(totalRecords3, Is.EqualTo(0));
            Assert.That(results1.ToArray()[0].UserName, Is.EqualTo(users1[2].UserName));
            Assert.That(results2.ToArray()[0].UserName, Is.EqualTo(users2[2].UserName));
        }

        [Test]
        public void GivenValidUsersWhenGetAllUsersThenResultsSuccess()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 0;
            var preFix = Guid.NewGuid().ToString("N");
            
            var users = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var userName = preFix + "_" + i;
                const string Password = "Password1!";
                WebSecurity.CreateUserAndAccount(userName, Password, null, true);
                users.Add(userName);
            }

            // act
            int totalRecords;
            var results = Membership.GetAllUsers(PageIndex, PageSize, out totalRecords);

            // assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(PageSize));
            Assert.That(results[users[0]].UserName, Is.Not.Null);
            Assert.That(totalRecords, Is.EqualTo(10));
        }

        [Test]
        public void GivenAValidUserWhenGetUserByIdThenReturnUserSuccess()
        {
            // arrange
            string prefix;
            var testUser = Helper.WithUser(out prefix);
            var membershipUser = Membership.GetUser(testUser.UserName);

            // act
            var user = Membership.GetUser(membershipUser.ProviderUserKey, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.StringStarting(prefix));
        }

        [Test]
        public void GivenAValidUserWhenGetUserNameByEmailThenReturnUserNameSuccess()
        {
            // arrange
            string prefix;
            var testUser = Helper.WithUser(out prefix);

            // act
            var userName = Membership.GetUserNameByEmail(testUser.Email);

            // assert
            Assert.That(userName, Is.Not.Null);
            Assert.That(userName, Is.EqualTo(testUser.UserName));
        }
    }
}