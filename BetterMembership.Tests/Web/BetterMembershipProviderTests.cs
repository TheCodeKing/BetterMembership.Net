namespace BetterMembership.Tests.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration.Provider;

    using BetterMembership.Web;

    using NUnit.Framework;

    [TestFixture]
    public class BetterMembershipProviderTests
    {
        [Test]
        public void GivenNoRequiresUniqueEmailAttributeWhenInitializeThenRequiresUniqueEmailIsFalse()
        {
            // arrange
            var testClass = new BetterMembershipProvider();
            var config = new NameValueCollection();

            // act
            testClass.Initialize("name", config);

            // assert
            Assert.That(testClass.RequiresUniqueEmail, Is.False);
        }

        [Test]
        public void GivenProviderWhenGetNumberOfUsersOnlineThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(() => provider.GetNumberOfUsersOnline());
        }

        [Test]
        public void GivenProviderWhenGetPasswordThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(() => provider.GetPassword("userName", "answer"));
        }


        [Test]
        public void GivenProviderWhenRequiresQuestionAndAnswerThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act
            var result = provider.RequiresQuestionAndAnswer;

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenProviderWhenGetUserByKeyWithInvalidKeyThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<ArgumentException>(() => provider.GetUser((object)"NotInteger", true));
        }

        [Test]
        public void GivenProviderWhenGetUserByKeyWithUsersOnlineFlagThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(() => provider.GetUser(1, true));
        }

        [Test]
        public void GivenProviderWhenGetUserWithUsersOnlineFlagThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(() => provider.GetUser("username", true));
        }

        [Test]
        public void GivenRequiresEmailWithoutEmailColumnWhenInitializeThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();
            var config = new NameValueCollection { { "requiresUniqueEmail", "true" } };

            // act // assert
            Assert.Throws<ProviderException>(() => provider.Initialize("name", config));
        }

        [Test]
        public void GivenRequiresQuestionAndAnswerAttributeWhenInitializeThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();
            var config = new NameValueCollection { { "requiresQuestionAndAnswer", "true" } };

            // act // assert
            Assert.Throws<ProviderException>(() => provider.Initialize("name", config));
        }

        [Test]
        public void GivenRequiresUniqueEmailWhenInitializeThenRequiresUniqueEmailIsCorrectValue()
        {
            // arrange
            var testClass = new BetterMembershipProvider();
            var config = new NameValueCollection { { "requiresUniqueEmail", "true" }, { "userEmailColumn", "Email" } };

            // act
            testClass.Initialize("name", config);

            // assert
            Assert.That(testClass.RequiresUniqueEmail, Is.True);
        }
    }
}