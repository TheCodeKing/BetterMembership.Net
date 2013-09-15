namespace BetterMembership.Tests.Extensions
{
    using System.Collections.Specialized;

    using BetterMembership.Extensions;

    using NUnit.Framework;

    [TestFixture]
    public class NameValueCollectionExtensionTests
    {
        private const string TestKey = "testKey";

        [Test]
        public void GivenCollectionWithKeyAsBooleanWhenGetBooleanThenReturnValue()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "false" } };

            // act
            var result = testClass.GetBoolean(TestKey, true);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenCollectionWithKeyAsEmtpyStringWhenGetBooleanThenReturnDefault()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, string.Empty } };

            // act
            var result = testClass.GetBoolean(TestKey, true);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenCollectionWithKeyAsEmtpyStringWhenGetIntegerThenReturnDefault()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, string.Empty } };

            // act
            var result = testClass.GetInteger(TestKey, 3);

            // assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GivenCollectionWithKeyAsIntegerWhenGetIntegerThenReturnValue()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "9" } };

            // act
            var result = testClass.GetInteger(TestKey, 3);

            // assert
            Assert.That(result, Is.EqualTo(9));
        }

        [Test]
        public void GivenCollectionWithKeyAsNotBooleanAndNoDefaulWhenGetBooleanThenReturnFalse()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "xx" } };

            // act
            var result = testClass.GetBoolean(TestKey);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenCollectionWithKeyAsNotBooleanWhenGetBooleanThenReturnDefault()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "xx" } };

            // act
            var result = testClass.GetBoolean(TestKey, true);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenCollectionWithKeyAsNotIntegerAndNoDefaulWhenGetIntegerThenReturnZero()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "xx" } };

            // act
            var result = testClass.GetInteger(TestKey);

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GivenCollectionWithKeyAsNotIntegerWhenGetIntegerThenReturnDefault()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "xx" } };

            // act
            var result = testClass.GetInteger(TestKey, 3);

            // assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GivenCollectionWithKeyWhenContainsKeyThenReturnTrue()
        {
            // arrange
            var testClass = new NameValueCollection { { TestKey, "value" } };

            // act
            var result = testClass.ContainsKey(TestKey);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenCollectionWithoutKeyWhenContainsKeyThenReturnFalse()
        {
            // arrange
            var testClass = new NameValueCollection();

            // act
            var result = testClass.ContainsKey(TestKey);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenNullCollectionWhenContainsKeyThenReturnFalse()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.ContainsKey(TestKey);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenNullCollectionWhenGetBooleanThenReturnDefault()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetBoolean(TestKey, true);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenNullCollectionWhenGetBooleanWihtoutDefaultThenReturnFalse()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetBoolean(TestKey);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenNullCollectionWhenGetIntegerThenReturnDefault()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetInteger(TestKey, 3);

            // assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GivenNullCollectionWhenGetIntegerWihtoutDefaultThenReturnZero()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetInteger(TestKey);

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GivenNullCollectionWhenGetStringWihtoutDefaultThenReturnNull()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetString(TestKey);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenNullCollectionWithNoDefaultWhenGetIntegerThenReturnZero()
        {
            // arrange
            var testClass = (NameValueCollection)null;

            // act
            var result = testClass.GetInteger(TestKey);

            // assert
            Assert.That(result, Is.EqualTo(0));
        }
    }
}