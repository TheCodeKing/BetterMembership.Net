namespace BetterMembership.IntegrationTests
{
    using System.Web.Security;

    using BetterMembership.Dummy;
    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Utils;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class DefaultProviderSwitcherTest
    {
        [SetUp]
        public void SetUp()
        {
            Helper.ClearDownDatabaseTables();
        }

        [Test]
        public void GivenDefaultProviderSwitcherWhenDisposedThenDefaultProviderIsRevertedToOriginalDefault()
        {
            // arrange
            var originalprovider = Membership.Provider;

            // act
            using (new DefaultProviderSwitcher(DummyMembershipProvider.Default))
            {
            }

            // assert
            Assert.That(Membership.Provider, Is.EqualTo(originalprovider));
        }

        [Test]
        public void GivenDefaultProviderSwitcherWhenProviderInitializedThenLeftInValidState()
        {
            // arrange
            var provider1 = Membership.Provider;

            // act
            var provider2 = Membership.Provider;

            // assert
            Assert.That(provider1, Is.EqualTo(provider2));
        }

        [Test]
        public void GivenMembershipProviderWhenSwitchedThenDefaultIsNewProvider()
        {
            // arrange // act
            using (new DefaultProviderSwitcher(DummyMembershipProvider.Default))
            {
                // assert
                Assert.That(Membership.Provider, Is.EqualTo(DummyMembershipProvider.Default));
            }
        }

        [Test]
        public void GivenWebSecurityThenInitializesProvidersThenSuccess()
        {
            // arrange //act //assert
            WebSecurity.Logout();
        }
    }
}