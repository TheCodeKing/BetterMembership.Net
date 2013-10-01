namespace BetterMembership.IntegrationTests.ProfileProvider
{
    using System;
    using System.Web.Profile;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetAllProfilesTests : BaseProfileProviderTests
    {
        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenGetAllProfilesThenProfilesReturned(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 2;
            const int PageIndex = 1;
            string prefix1;
            const int UserGroup1Count = 8;
            var users1 = memProvider.WithConfirmedUsers(UserGroup1Count, out prefix1).Value;

            // act
            int totalRecords1;
            var results1 = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords1);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(PageSize));
            Assert.That(totalRecords1, Is.EqualTo(UserGroup1Count));
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
            var results1 = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords1);

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
            var results1 = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords1);

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
            var results1 = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords1);
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
    }
}