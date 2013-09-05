namespace BetterMembership.IntegrationTests
{
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class BetterMembershipProviderTests
    {
        private const int ConfiguredAllowedPasswordAttempts = 3;

        private const int ConfiguredPasswordLockoutTimeoutInSeconds = 1;

        [SetUp]
        public void SetUp()
        {
            Helper.ClearDownDatabaseTables();

            HttpContext.Current.SetupCurrentHttpContext();
        }

        [Test]
        public void GivenConfirmedUserWhenExactlyMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts);

            // act
            var user = Membership.GetUser(testUser.UserName);
            var webSecurityIsLockedOut = WebSecurity.IsAccountLockedOut(
                testUser.UserName, ConfiguredAllowedPasswordAttempts, ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(webSecurityIsLockedOut, Is.False);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserByIdThenUserIsFound()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser();
            var membershipUser = Membership.GetUser(testUser.UserName);

            // act
            var user = Membership.GetUser(membershipUser.ProviderUserKey, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserNameByEmailThenReturnUserName()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser();

            // act
            var userName = Membership.GetUserNameByEmail(testUser.Email);

            // assert
            Assert.That(userName, Is.Not.Null);
            Assert.That(userName, Is.EqualTo(testUser.UserName));
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserThenReturnMembershipUser()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser();

            // act
            var user = Membership.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserThenUserIsApproved()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser();

            // act
            var user = Membership.GetUser(testUser.UserName);

            // assert
            Assert.That(user.IsApproved, Is.True);
        }

        [Test]
        public void GivenConfirmedUserWhenLoginThenUserCanAuthenticate()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser();

            // act
            var webSecurityResult = WebSecurity.Login(testUser.UserName, testUser.Password);
            var membershipResult = Membership.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.True);
            Assert.That(membershipResult, Is.True);
        }

        [Test]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            ()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            Thread.Sleep(1000 * ConfiguredPasswordLockoutTimeoutInSeconds);
            var user = Membership.GetUser(testUser.UserName);
            var webSecurityIsLockedOut = WebSecurity.IsAccountLockedOut(
                testUser.UserName, ConfiguredAllowedPasswordAttempts, ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(webSecurityIsLockedOut, Is.False);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountLockedOutSuccess()
        {
            // arrange
            var testUser = Helper.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            var user = Membership.GetUser(testUser.UserName);
            var webSecurityIsLockedOut = WebSecurity.IsAccountLockedOut(
                testUser.UserName, ConfiguredAllowedPasswordAttempts, ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(webSecurityIsLockedOut, Is.True);
            Assert.That(user.IsLockedOut, Is.True);
        }

        [Test]
        public void GivenConfirmedUsersWhenFindUsersbyEmailThenPageResultsSuccessfully()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 1;
            string preFix1;
            string preFix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = Helper.WithConfirmedUsers(UserGroup1Count, out preFix1);
            var users2 = Helper.WithConfirmedUsers(UserGroup2Count, out preFix2);

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
        public void GivenConfirmedUsersWhenFindUsersbyUserNameThenPageResultsSuccessFully()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 1;
            string preFix1;
            string preFix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = Helper.WithConfirmedUsers(UserGroup1Count, out preFix1);
            var users2 = Helper.WithConfirmedUsers(UserGroup2Count, out preFix2);

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
        public void GivenConfirmedUsersWhenGetAllUsersThenReturnFirstPageUsersSuccessfully()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 0;
            string preFix1;
            const int UserGroup1Count = 10;
            var users1 = Helper.WithConfirmedUsers(UserGroup1Count, out preFix1);

            // act
            int totalRecords;
            var results = Membership.GetAllUsers(PageIndex, PageSize, out totalRecords);

            // assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(PageSize));
            Assert.That(results.First().UserName, Is.EqualTo(users1.First().UserName));
            Assert.That(totalRecords, Is.EqualTo(10));
        }

        [Test]
        public void GivenMultipleProvidersWhenNonDefaultProviderUsedForCreateUserAndAccountThenUserAccountIsCreated()
        {
            // arrange
            var testUser = Helper.WithUnregisteredUser();
            var provider = (ExtendedMembershipProvider)Membership.Providers["nonDefaultProvider"];

            // act
            var result = provider.CreateUserAndAccount(testUser.UserName, testUser.Password, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenUnConfirmedUserWhenGetUserThenMembershipUserIsReturned()
        {
            // arrange
            var testUser = Helper.WithUnConfirmedUser();

            // act
            var user = Membership.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [Test]
        public void GivenUnConfirmedUserWhenGetUserThenUserIsNotApproved()
        {
            // arrange
            var testUser = Helper.WithUnConfirmedUser();

            // act
            var user = Membership.GetUser(testUser.UserName);

            // assert    
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(user.IsApproved, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate()
        {
            // arrange
            var testUser = Helper.WithUnConfirmedUser();

            // act
            var webSecurityResult = WebSecurity.Login(testUser.UserName, testUser.Password);
            var membershipResult = Membership.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.False);
            Assert.That(membershipResult, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut()
        {
            // arrange
            var testUser =
                Helper.WithUnConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            var user = Membership.GetUser(testUser.UserName);
            var webSecurityIsLockedOut = WebSecurity.IsAccountLockedOut(
                testUser.UserName, ConfiguredAllowedPasswordAttempts, ConfiguredPasswordLockoutTimeoutInSeconds);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(webSecurityIsLockedOut, Is.False);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenUnregisteredUserWhenCreateUserAndAccountThenUserAccountIsCreated()
        {
            // arrange
            var testUser = Helper.WithUnregisteredUser();

            // act
            var id = WebSecurity.CreateUserAndAccount(testUser.UserName, testUser.Password, null, true);

            // assert
            Assert.That(id, Is.Not.Null);
        }
    }
}