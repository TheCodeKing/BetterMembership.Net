namespace BetterMembership.IntegrationTests.ProfileProvider
{
    using System;
    using System.Web.Profile;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class DeleteProfilesTests : BaseProfileProviderTests
    {
        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenDeleteProfilesThenProfilesDeletedSuccessfully(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            const int PageSize = 10;
            const int PageIndex = 0;
            string prefix1;
            const int UserGroupCount = PageSize;
            int totalRecords;
            var users = memProvider.WithConfirmedUsers(UserGroupCount, out prefix1).Value;
            var profiles = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords);

            // act
            testClass.DeleteProfiles(profiles);

            // assert
            var checkProfiles = testClass.GetAllProfiles(ProfileAuthenticationOption.All, PageIndex, PageSize, out totalRecords);
            Assert.That(checkProfiles, Is.Not.Null);
            Assert.That(checkProfiles.Count, Is.EqualTo(0));
            Assert.That(totalRecords, Is.EqualTo(0));

            var checkUsers = memProvider.GetAllUsers(0, 10, out totalRecords);
            Assert.That(checkUsers, Is.Not.Null);
            Assert.That(checkUsers.Count, Is.EqualTo(0));
            Assert.That(totalRecords, Is.EqualTo(0));
        }
    }
}