namespace BetterMembership.IntegrationTests.SqlClient.Membership.WithoutEmailColumn
{
    using System;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class MembershipWithoutEmailColumn
    {
        private const int ConfiguredAllowedPasswordAttempts = 3;

        private const int ConfiguredPasswordLockoutTimeoutInSeconds = 1;

        private ExtendedMembershipProvider provider;

        [SetUp]
        public void SetUp()
        {
            Helper.ClearDownDatabaseTables();

            HttpContext.Current.SetupCurrentHttpContext();

            this.provider = Membership.Providers["defaultProviderWithoutEmailColumn"] as ExtendedMembershipProvider;
        }

        [Test]
        public void GivenConfirmedUserWhenExactlyMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut()
        {
            // arrange
            var testUser =
                this.provider.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts);

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserByIdThenUserIsFound()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();
            var membershipUser = this.provider.GetUser(testUser.UserName, false);

            // act
            var user = this.provider.GetUser(membershipUser.ProviderUserKey, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserNameByEmailThenReturnUserName()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();

            // act // assert
            Assert.Throws<ProviderException>(() => this.provider.GetUserNameByEmail(testUser.Email));
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserThenReturnMembershipUser()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void GivenConfirmedUserWhenGetUserThenUserIsApproved()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user.IsApproved, Is.True);
        }

        [Test]
        public void GivenConfirmedUserWhenLoginThenUserCanAuthenticate()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();

            // act
            var result = this.provider.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void
            GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsAndWaitLongerThanPasswordTimeoutThenAccountIsNotLockedOut
            ()
        {
            // arrange
            var testUser =
                this.provider.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            Thread.Sleep(1000 * ConfiguredPasswordLockoutTimeoutInSeconds + 1);
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountLockedOutSuccess()
        {
            // arrange
            var testUser =
                this.provider.WithConfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsLockedOut, Is.True);
        }

        [Test]
        public void GivenConfirmedUserWhenNoEmailColumnUpdateUserThenUserIsUpdated()
        {
            // arrange
            var testUser = this.provider.WithConfirmedUser();
            var user = this.provider.GetUser(testUser.UserName, false);
            user.IsApproved = false;

            // act
            this.provider.UpdateUser(user);

            // assert 
            var reloadUser = this.provider.GetUser(testUser.UserName, false);
            Assert.That(reloadUser, Is.Not.Null);
            Assert.That(reloadUser.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(reloadUser.IsApproved, Is.False);
        }

        [Test]
        public void GivenConfirmedUsersWhenFindUsersbyEmailWithoutEmailColumnThenProviderException()
        {
            // arrange
            const int PageSize = 2;
            const int PageIndex = 1;
            string preFix1;
            const int UserGroup1Count = 8;
            var users1 = this.provider.WithConfirmedUsers(UserGroup1Count, out preFix1);

            // act // assert
            int totalRecords1;
            Assert.Throws<ProviderException>(() => this.provider.FindUsersByEmail(preFix1, PageIndex, PageSize, out totalRecords1));
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
            var users1 = this.provider.WithConfirmedUsers(UserGroup1Count, out preFix1);
            var users2 = this.provider.WithConfirmedUsers(UserGroup2Count, out preFix2);

            // act
            int totalRecords1;
            int totalRecords2;
            int totalRecords3;
            var results1 = this.provider.FindUsersByName(preFix1, PageIndex, PageSize, out totalRecords1);
            var results2 = this.provider.FindUsersByName(preFix2, PageIndex, PageSize, out totalRecords2);
            var results3 = this.provider.FindUsersByName("missing", PageIndex, PageSize, out totalRecords3);

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
            var users1 = this.provider.WithConfirmedUsers(UserGroup1Count, out preFix1);

            // act
            int totalRecords;
            var results = this.provider.GetAllUsers(PageIndex, PageSize, out totalRecords);

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
            var testUser = this.provider.WithUnregisteredUser();

            // act
            var result = this.provider.CreateUserAndAccount(testUser.UserName, testUser.Password, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenProviderWithoutEmailDefinedWhenUserEmailUpdatedThenNotSupportedException()
        {
            // arrange
            const string NewEmail = "newemail@test.com";
            var testUser = this.provider.WithUnregisteredUser();
            MembershipCreateStatus status;
            var user = this.provider.CreateUser(
                testUser.UserName, testUser.Password, null, null, null, true, null, out status);

            // act // assert
            Assert.Throws<NotSupportedException>(() => user.Email = NewEmail);
        }

        [Test]
        public void GivenUnConfirmedUserWhenGetUserThenMembershipUserIsReturned()
        {
            // arrange
            var testUser = this.provider.WithUnconfirmedUser();

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [Test]
        public void GivenUnConfirmedUserWhenGetUserThenUserIsNotApproved()
        {
            // arrange
            var testUser = this.provider.WithUnconfirmedUser();

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert    
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(user.IsApproved, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate()
        {
            // arrange
            var testUser = this.provider.WithUnconfirmedUser();

            // act
            var result = this.provider.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut()
        {
            // arrange
            var testUser =
                this.provider.WithUnconfirmedUser().WithInvalidPasswordAttempts(ConfiguredAllowedPasswordAttempts + 1);

            // act
            var user = this.provider.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(user.IsLockedOut, Is.False);
        }

        [Test]
        public void GivenUnConfirmedUserWhenUpdateUserThenUserIsUpdated()
        {
            // arrange
            var testUser = this.provider.WithUnconfirmedUser();
            var user = this.provider.GetUser(testUser.UserName, false);
            user.IsApproved = true;

            // act
            this.provider.UpdateUser(user);

            // assert 
            var reloadUser = this.provider.GetUser(testUser.UserName, false);
            Assert.That(reloadUser, Is.Not.Null);
            Assert.That(reloadUser.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(reloadUser.IsApproved, Is.True);
        }
    }
}