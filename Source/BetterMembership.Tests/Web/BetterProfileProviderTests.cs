namespace BetterMembership.Tests.Web
{
    using System;
    using System.Web.Profile;

    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class BetterProfileProviderTests
    {
        [TestCase(ProfileAuthenticationOption.All)]
        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenConfirmedUsersWhenDeleteInactiveProfilesThenNotSupportedException(ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(
                () => testClass.DeleteInactiveProfiles(option, DateTime.MinValue));
        }

        [TestCase(ProfileAuthenticationOption.All)]
        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenConfirmedUsersWhenFindInactiveProfilesByUserNameThenNotSupportedException(ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            int totalRecords;
            Assert.Throws<NotSupportedException>(
                () =>
                testClass.FindInactiveProfilesByUserName(
                    option, "value", DateTime.MinValue, 0, 10, out totalRecords));
        }

        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenConfirmedUsersWhenFindUsersbyUserNameThenThrowNotSupportedException(
            ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            int totalRecords1;
            Assert.Throws<NotSupportedException>(
                () => testClass.FindProfilesByUserName(option, "value", 0, 10, out totalRecords1));
        }

        [TestCase(ProfileAuthenticationOption.All)]
        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenConfirmedUsersWhenGetAllInactiveProfilesThenNotSupportedException(ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            int totalRecords;
            Assert.Throws<NotSupportedException>(
                () =>
                testClass.GetAllInactiveProfiles(
                    option, DateTime.MinValue, 0, 10, out totalRecords));
        }

        [TestCase(ProfileAuthenticationOption.All)]
        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenConfirmedUsersWhenGetNumberOfInactiveProfilesThenNotSupportedException(ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(
                () => testClass.GetNumberOfInactiveProfiles(option, DateTime.MinValue));
        }

        [TestCase(ProfileAuthenticationOption.Anonymous)]
        [TestCase(ProfileAuthenticationOption.Authenticated)]
        public void GivenProfileProviderWhenGetAllProfilesThenThrowNotSupportedException(ProfileAuthenticationOption option)
        {
            // arrange
            var testClass = new BetterProfileProvider();

            // act // assert
            int totalRecords;
            Assert.Throws<NotSupportedException>(
                () => testClass.GetAllProfiles(option, 0, 10, out totalRecords));
        }
    }
}