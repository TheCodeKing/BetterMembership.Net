namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenGetUserByIdThenUserIsFound(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;
            var membershipUser = testClass.GetUser(testUser.UserName, false);

            // act
            var user = testClass.GetUser(membershipUser.ProviderUserKey, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenGetUserThenReturnMembershipUser(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenGetUserThenUserIsApproved(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user.IsApproved, Is.True);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenGetUserThenMembershipUserIsReturned(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenGetUserThenUserIsNotApproved(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert    
            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
            Assert.That(user.IsApproved, Is.False);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenGetUserThenMembershipUserIsNull(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert
            Assert.That(user, Is.Null);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenGetUserByIdThenMembershipUserIsNull(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);

            // act
            var user = testClass.GetUser((object)int.MaxValue, false);

            // assert
            Assert.That(user, Is.Null);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnReqisteredUserWhenGetUserThenUserIsNull(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;

            // act
            var user = testClass.GetUser(testUser.UserName, false);

            // assert 
            Assert.That(user, Is.Null);
        }
    }
}