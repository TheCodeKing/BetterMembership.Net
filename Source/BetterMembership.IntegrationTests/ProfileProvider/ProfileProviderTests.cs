namespace BetterMembership.IntegrationTests.ProfileProvider
{
    using System;
    using System.Web.Profile;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ProfileProviderTests : BaseProfileProviderTests
    {
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