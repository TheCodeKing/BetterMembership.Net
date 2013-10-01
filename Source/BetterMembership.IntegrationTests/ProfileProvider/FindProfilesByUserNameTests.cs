namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System;
    using System.Web.Profile;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class FindProfilesByUserNameTests : BaseProfileProviderTests
    {
        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindProfilesByUserNameThenProfilesReturned(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            string prefix1;
            string prefix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = memProvider.WithConfirmedUsers(UserGroup1Count, out prefix1).Value;
            var users2 = memProvider.WithConfirmedUsers(UserGroup2Count, out prefix2).Value;

            // act
            int totalRecords1;
            int totalRecords2;
            int totalRecords3;
            var results1 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, prefix1, PageIndex, PageSize, out totalRecords1);
            var results2 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, prefix2, PageIndex, PageSize, out totalRecords2);
            var results3 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, "missing", PageIndex, PageSize, out totalRecords3);

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

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameThenFirstPageCorrect(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 0;
            const int TotalUsers = 3;
            string prefix1;
            var users1 = memProvider.WithConfirmedUsers(TotalUsers, out prefix1).Value;

            // act
            int totalRecords1;
            var results1 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, prefix1, PageIndex, PageSize, out totalRecords1);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(PageSize));
            Assert.That(totalRecords1, Is.EqualTo(TotalUsers));
            Assert.That(results1.ToArray()[0].UserName, Is.EqualTo(users1[0].UserName));
            Assert.That(results1.ToArray()[1].UserName, Is.EqualTo(users1[1].UserName));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameThenLastPageCorrect(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            const int TotalUsers = 3;
            string prefix1;
            var users1 = memProvider.WithConfirmedUsers(TotalUsers, out prefix1).Value;

            // act
            int totalRecords1;
            var results1 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, prefix1, PageIndex, PageSize, out totalRecords1);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(1));
            Assert.That(totalRecords1, Is.EqualTo(TotalUsers));
            Assert.That(results1.ToArray()[0].UserName, Is.EqualTo(users1[2].UserName));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameThenReturnProfileCorrect(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            const int TotalUsers = 3;
            string prefix1;
            var users1 = memProvider.WithConfirmedUsers(TotalUsers, out prefix1).Value;

            // act
            int totalRecords1;
            var results1 = testClass.FindProfilesByUserName(ProfileAuthenticationOption.All, prefix1, PageIndex, PageSize, out totalRecords1);
            ProfileInfo info = results1.ToArray()[0];

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(1));
            Assert.That(totalRecords1, Is.EqualTo(TotalUsers));
            Assert.That(info.UserName, Is.EqualTo(users1[2].UserName));
            Assert.That(info.Size, Is.EqualTo(0));
            Assert.That(info.IsAnonymous, Is.True);
            Assert.That(info.LastActivityDate, Is.EqualTo(DateTime.MinValue));
            Assert.That(info.LastUpdatedDate, Is.EqualTo(DateTime.MinValue));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameWithAnonymousThenThrowNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            const int TotalUsers = 3;
            string prefix1;
            var users1 = memProvider.WithConfirmedUsers(TotalUsers, out prefix1).Value;

            // act // assert
            int totalRecords1;
            Assert.Throws<NotSupportedException>(() => testClass.FindProfilesByUserName(ProfileAuthenticationOption.Anonymous, prefix1, PageIndex, PageSize, out totalRecords1));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameWithAuthenticatedThenThrowNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            const int TotalUsers = 3;
            string prefix1;
            var users1 = memProvider.WithConfirmedUsers(TotalUsers, out prefix1).Value;

            // act // assert
            int totalRecords1;
            Assert.Throws<NotSupportedException>(() => testClass.FindProfilesByUserName(ProfileAuthenticationOption.Authenticated, prefix1, PageIndex, PageSize, out totalRecords1));
        }
    }
}