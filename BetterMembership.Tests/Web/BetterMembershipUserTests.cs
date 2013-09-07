namespace BetterMembership.Tests.Web
{
    using System;

    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class BetterMembershipUserTests
    {
        [Test]
        public void GivenMembershipUserWhenIsOnlineThenNotSupportedException()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true);

            // act // assert
            var result = false;
            Assert.Throws<NotSupportedException>(() => result = user.IsOnline);
        }

        [Test]
        public void GivenMembershipUserWhenGetCommentThenEmptyString()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true);

            // act
            var result = user.Comment;

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenMembershipUserWhenCommentThenNotSupportedException()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true);

            // act // assert
            Assert.Throws<NotSupportedException>(() => user.Comment = "comment");
        }

        [Test]
        public void GivenMembershipUserWithNoEmailSupportWhenEmailIsSetThenNotSupportedException()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false);

            // act // assert
            Assert.Throws<NotSupportedException>(() => user.Email = "email");
        }

        [Test]
        public void GivenMembershipUserWithEmailSupportWhenEmailIsSetThenSetSuccess()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true);

            // act
            user.Email = "email";

            // assert
            Assert.That(user.Email, Is.EqualTo("email"));
        }

        [Test]
        public void GivenMembershipUserWhenIsLockedOutThenInvokeDelegate()
        {
            // arrange
            var user = new BetterMembershipUser(
                "DefaultProvider",
                "userName",
                1,
                null,
                null,
                null,
                true,
                () => true,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                true);

            // act
            var result = user.IsLockedOut;

            // assert
            Assert.That(result, Is.True);
        }
    }
}