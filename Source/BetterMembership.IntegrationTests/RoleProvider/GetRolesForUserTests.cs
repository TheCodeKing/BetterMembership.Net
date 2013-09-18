namespace BetterMembership.IntegrationTests.RoleProvider
{
    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetRolesForUserTests : BaseRoleMembershipTests
    {
        [TestCase(SqlClientCeRoleProviderWithEmail, SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeRoleProviderWithoutEmail, SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeRoleProviderWithUniqueEmail, SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientRoleProviderWithEmail, SqlClientProviderWithEmail)]
        [TestCase(SqlClientRoleProviderWithoutEmail, SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientRoleProviderWithUniqueEmail, SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUserWithRolesWhenGetRolesForUserThenRolesReturned(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var testUser = memProvider.WithConfirmedUser().Value;
            var testRoles = testClass.WithUserInRoles(testUser.UserName, "Role1", "Role2");

            // act
            var roles = testClass.GetRolesForUser(testUser.UserName);

            // assert
            Assert.That(roles.Length, Is.EqualTo(testRoles.Length));
            Assert.That(roles[0], Is.EqualTo(testRoles[0]));
            Assert.That(roles[1], Is.EqualTo(testRoles[1]));
        }

        [TestCase(SqlClientCeRoleProviderWithEmail, SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeRoleProviderWithoutEmail, SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeRoleProviderWithUniqueEmail, SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientRoleProviderWithEmail, SqlClientProviderWithEmail)]
        [TestCase(SqlClientRoleProviderWithoutEmail, SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientRoleProviderWithUniqueEmail, SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUserWithoutRolesWhenGetRolesForUserThenEmptyRolesReturned(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var testUser = memProvider.WithConfirmedUser().Value;

            // act
            var roles = testClass.GetRolesForUser(testUser.UserName);

            // assert
            Assert.That(roles.Length, Is.EqualTo(0));
        }
    }
}