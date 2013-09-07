namespace BetterMembership.IntegrationTests.MembershipProvider
{
    using System.Linq;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetAllUsersTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderNameWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderNameWithoutEmail)]
        public void GivenConfirmedUsersWhenGetAllUsersThenReturnFirstPageUsersSuccessfully(string providerName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            const int PageSize = 2;
            const int PageIndex = 0;
            string preFix1;
            const int UserGroup1Count = 10;
            var users1 = testClass.WithConfirmedUsers(UserGroup1Count, out preFix1).Value;

            // act
            int totalRecords;
            var results = testClass.GetAllUsers(PageIndex, PageSize, out totalRecords);

            // assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(PageSize));
            Assert.That(results.First().UserName, Is.EqualTo(users1.First().UserName));
            Assert.That(totalRecords, Is.EqualTo(10));
        }
    }
}