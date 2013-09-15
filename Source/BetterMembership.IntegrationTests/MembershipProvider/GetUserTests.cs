namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetUserTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
        public void GivenUnregisteredUserWhenGetUserByIdThenMembershipUserIsNull(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);

            // act
            var user = testClass.GetUser((object)int.MaxValue, false);

            // assert
            Assert.That(user, Is.Null);
        }

        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderNameWithoutEmail)]
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