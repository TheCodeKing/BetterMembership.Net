namespace BetterMembership.Tests.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration.Provider;

    using BetterMembership.Web;

    using NUnit.Framework;

    using StructureMap.AutoMocking;

    [TestFixture]
    public class BetterMembershipProviderTests
    {
        [Test]
        public void
            GivenEmailStrengthRegularExpressionConfigWhenDefaultEmailStrengthRegularExpressionThenCorrectValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "emailStrengthRegularExpression", ".*" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.EmailStrengthRegularExpression;

            // assert
            Assert.That(result, Is.EqualTo(".*"));
        }

        [Test]
        public void
            GivenEmptyEmailStrengthRegularExpressionConfigWhenemailStrengthRegularExpressionThenEmptytValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "emailStrengthRegularExpression", string.Empty } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.EmailStrengthRegularExpression;

            // assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GivenEmptyMaxEmailLengthConfigWhenMaxEmailLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxEmailLength", string.Empty } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxEmailLength;

            // assert
            Assert.That(result, Is.EqualTo(254));
        }

        [Test]
        public void GivenEmptyMaxPasswordLengthConfigWhenMaxPasswordLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxPasswordLength", string.Empty } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(128));
        }

        [Test]
        public void GivenEmptyMaxUserNameLengthConfigWhenMaxUserNameLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxUserNameLength", string.Empty } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxUserNameLength;

            // assert
            Assert.That(result, Is.EqualTo(56));
        }

        [Test]
        public void
            GivenInvalidEmailStrengthRegularExpressionConfigWhenemailStrengthRegularExpressionThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "emailStrengthRegularExpression", "(.*" } };

            // act // asert
            Assert.Throws<ProviderException>(() => testClass.Initialize("name", config));
        }

        [Test]
        public void GivenInvalidMaxEmailLengthConfigWhenMaxEmailLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxEmailLength", "x" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxEmailLength;

            // assert
            Assert.That(result, Is.EqualTo(254));
        }

        [Test]
        public void GivenInvalidMaxPasswordLengthConfigWhenMaxPasswordLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxPasswordLength", "x" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(128));
        }

        [Test]
        public void GivenInvalidMaxUserNameLengthConfigWhenMaxUserNameLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxUserNameLength", "x" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxUserNameLength;

            // assert
            Assert.That(result, Is.EqualTo(56));
        }

        [Test]
        public void
            GivenInvalidMinRequiredNonalphanumericCharactersConfigWhenMinRequiredNonalphanumericCharactersThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "minRequiredNonalphanumericCharacters", "x" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredNonAlphanumericCharacters;

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GivenInvalidMinRequiredPasswordLengthConfigWhenMinRequiredPasswordLengthThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "minRequiredPasswordLength", "x" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GivenMaxEmailLengthConfigWhenMaxEmailLengthThenCorrectReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxEmailLength", "10" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxEmailLength;

            // assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GivenMaxPasswordLengthConfigWhenMaxPasswordLengthThenCorrectValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxPasswordLength", "10" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GivenMaxUserNameLengthConfigWhenMaxUserNameLengthThenCorrectValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "maxUserNameLength", "10" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MaxUserNameLength;

            // assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void
            GivenMinRequiredNonalphanumericCharactersConfigWhenMinRequiredNonalphanumericCharactersThenCorrectValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "minRequiredNonalphanumericCharacters", "2" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredNonAlphanumericCharacters;

            // assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GivenMinRequiredPasswordLengthConfigWhenMinRequiredPasswordLengthThenCorrectValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection { { "minRequiredPasswordLength", "2" } };

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void
            GivenNoMinRequiredNonalphanumericCharactersConfigWhenMinRequiredNonalphanumericCharactersThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection();

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredNonAlphanumericCharacters;

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void
            GivenNoMinRequiredPasswordLengthConfigWhenMinRequiredNonalphanumericCharactersThenDefaultValueReturned()
        {
            // arrange
            var testClass = new MoqAutoMocker<BetterMembershipProvider>().ClassUnderTest;
            var config = new NameValueCollection();

            testClass.Initialize("name", config);

            // act
            var result = testClass.MinRequiredPasswordLength;

            // assert
            Assert.That(result, Is.EqualTo(1));
        }

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
        public void GivenProviderWhenResetPasswordWithAnswerThenNotSupportedException()
        {
            // arrange
            var provider = new BetterMembershipProvider();

            // act // assert
            Assert.Throws<NotSupportedException>(() => provider.ResetPassword("username", "not null"));
        }

        [Test]
        public void GivenRequiresEmailWithoutEmailColumnWhenInitializeThenProviderException()
        {
            // arrange
            var provider = new BetterMembershipProvider();
            var config = new NameValueCollection
                             {
                                 { "requiresUniqueEmail", "true" },
                                 { "userEmailColumn", string.Empty }
                             };

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