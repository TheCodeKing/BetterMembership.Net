namespace BetterMembership.IntegrationTests.ProfileProvider
{
    using System;
    using System.Web.Profile;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ProfileProviderTests : BaseProfileProviderTests
    {
        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenGetNumberOfInactiveProfilesThenNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            string prefix1;
            const int UserGroupCount = 10;
            var users = memProvider.WithConfirmedUsers(UserGroupCount, out prefix1).Value;

            // act // assert
            Assert.Throws<NotSupportedException>(
                () => testClass.GetNumberOfInactiveProfiles(ProfileAuthenticationOption.All, DateTime.MinValue));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenFindInactiveProfilesByUserNameThenNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            string prefix1;
            const int PageSize = 10;
            const int PageIndex = 0;
            const int UserGroupCount = PageSize;
            var users = memProvider.WithConfirmedUsers(UserGroupCount, out prefix1).Value;

            // act // assert
            int totalRecords;
            Assert.Throws<NotSupportedException>(
                () =>
                testClass.FindInactiveProfilesByUserName(
                    ProfileAuthenticationOption.All, prefix1, DateTime.MinValue, PageIndex, PageSize, out totalRecords));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenGetAllInactiveProfilesThenNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            string prefix1;
            const int PageSize = 10;
            const int PageIndex = 0;
            const int UserGroupCount = PageSize;
            var users = memProvider.WithConfirmedUsers(UserGroupCount, out prefix1).Value;

            // act // assert
            int totalRecords;
            Assert.Throws<NotSupportedException>(
                () =>
                testClass.GetAllInactiveProfiles(
                    ProfileAuthenticationOption.All, DateTime.MinValue, PageIndex, PageSize, out totalRecords));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenDeleteInactiveProfilesThenNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            string prefix1;
            const int UserGroupCount = 10;
            var users = memProvider.WithConfirmedUsers(UserGroupCount, out prefix1).Value;

            // act // assert
            Assert.Throws<NotSupportedException>(
                () => testClass.DeleteInactiveProfiles(ProfileAuthenticationOption.All, DateTime.MinValue));
        }

        [Test]
        public void GivenConfirmedUserWhenEmailUpdatedThenProfileUserEmailIsUpdated()
        {
            // arrange
            var memProvider = this.WithMembershipProvider();
            var user = memProvider.WithConfirmedUser().Value;

            // act
            var profile = ProfileBase.Create(user.UserName);
            profile["Email"] = "new@email.com";
            profile.Save();

            var updatedProfile = ProfileBase.Create(user.UserName);

            // assert
            Assert.That(updatedProfile["Email"], Is.EqualTo("new@email.com"));
        }

        [Test]
        public void GivenConfirmedUserWhenProfileCreatedWithDefaultProviderThenProfileIsReturned()
        {
            // arrange
            var memProvider = this.WithMembershipProvider();
            var user = memProvider.WithConfirmedUser().Value;
            var actualUser = memProvider.GetUser(user.UserName, false);

            // act
            var profile = ProfileBase.Create(user.UserName);

            // assert
            Assert.That(profile.UserName, Is.EqualTo(user.UserName));
            Assert.That(profile["Email"], Is.EqualTo(user.Email));
            Assert.That(profile["UserId"], Is.EqualTo(actualUser.ProviderUserKey));
        }

        [Test]
        public void GivenUnregisteredUserWhenProfileCreatedWithDefaultProviderThenProfileDefaultValuesReturned()
        {
            // arrange
            var memProvider = this.WithMembershipProvider();
            var user = memProvider.WithUnregisteredUser().Value;

            // act
            var profile = ProfileBase.Create(user.UserName);

            // assert
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile["Email"], Is.EqualTo("nobody@nomail.com"));
            Assert.That(profile["UserId"], Is.EqualTo(0));
        }

        [Test]
        public void GivenUnregisteredUserWhenProfileCreatedWithDefaultProviderThenProfileIsReturnedAndSaveDoesNotError()
        {
            // arrange
            var memProvider = this.WithMembershipProvider();
            var user = memProvider.WithUnregisteredUser().Value;

            // act
            var profile = ProfileBase.Create(user.UserName);
            Assert.DoesNotThrow(profile.Save);

            // assert
            Assert.That(profile, Is.Not.Null);
        }
    }
}