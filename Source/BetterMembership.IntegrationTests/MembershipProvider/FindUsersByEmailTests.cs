namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Configuration.Provider;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class FindUsersByEmailTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUserWhenGetUserNameByEmailThenReturnUserName(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var testUser = testClass.WithConfirmedUser().Value;

            // act
            if (!testClass.HasEmailColumnDefined())
            {
                Assert.Throws<ProviderException>(() => testClass.GetUserNameByEmail(testUser.Email));
                return;
            }

            var userName = testClass.GetUserNameByEmail(testUser.Email);

            // assert
            Assert.That(userName, Is.Not.Null);
            Assert.That(userName, Is.EqualTo(testUser.UserName));
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenConfirmedUsersWhenFindUsersbyEmailThenPageResultsSuccessfully(string providerName)
        {
            // arrange
            var provider = this.WithProvider(providerName);
            const int PageSize = 2;
            const int PageIndex = 1;
            string prefix1;
            string prefix2;
            const int UserGroup1Count = 8;
            const int UserGroup2Count = 3;
            var users1 = provider.WithConfirmedUsers(UserGroup1Count, out prefix1).Value;
            var users2 = provider.WithConfirmedUsers(UserGroup2Count, out prefix2).Value;

            // act
            int totalRecords1;
            if (!provider.HasEmailColumnDefined())
            {
                Assert.Throws<ProviderException>(() => provider.FindUsersByEmail(prefix1, PageIndex, PageSize, out totalRecords1));
                return;
            }

            int totalRecords2;
            int totalRecords3;
            var results1 = provider.FindUsersByEmail(prefix1, PageIndex, PageSize, out totalRecords1);
            var results2 = provider.FindUsersByEmail(prefix2, PageIndex, PageSize, out totalRecords2);
            var results3 = provider.FindUsersByEmail("missing", PageIndex, PageSize, out totalRecords3);

            // assert
            Assert.That(results1, Is.Not.Null);
            Assert.That(results2, Is.Not.Null);
            Assert.That(results3, Is.Not.Null);
            Assert.That(results1.Count, Is.EqualTo(PageSize));
            Assert.That(results2.Count, Is.EqualTo(1));
            Assert.That(results3.Count, Is.EqualTo(0));
            Assert.That(totalRecords1, Is.EqualTo(UserGroup1Count));
            Assert.That(totalRecords2, Is.EqualTo(UserGroup2Count));
            Assert.That(totalRecords3, Is.EqualTo(0));
            Assert.That(results1.ToArray()[0].UserName, Is.EqualTo(users1[2].UserName));
            Assert.That(results2.ToArray()[0].UserName, Is.EqualTo(users2[2].UserName));
        }
    }
}