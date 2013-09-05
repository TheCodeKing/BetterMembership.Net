namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;

    using WebMatrix.WebData;

    internal static class MembershipExtensions
    {
        private const string DefaultPassword = "Password1!";

        public static TestUser WithConfirmedUser(this ExtendedMembershipProvider provider)
        {
            var prefix = Guid.NewGuid().ToString("N");
            var userName = prefix;
            var email = prefix + "@test.com";
            provider.CreateUserAndAccount(
                userName, DefaultPassword, new Dictionary<string, object> { { "Email", email } });
            return new TestUser(userName, email, DefaultPassword);
        }

        public static IList<TestUser> WithConfirmedUsers(
            this ExtendedMembershipProvider provider, int count, out string prefix)
        {
            prefix = Guid.NewGuid().ToString("N");
            var users = new List<TestUser>();
            for (var i = 0; i < count; i++)
            {
                var userName = prefix + "_" + i;
                var email = prefix + i + "@test.com";

                provider.CreateUserAndAccount(
                    userName, DefaultPassword, new Dictionary<string, object> { { "Email", email } });
                users.Add(new TestUser(userName, email, DefaultPassword));
            }

            return users;
        }

        public static TestUser WithUnconfirmedUser(this ExtendedMembershipProvider provider)
        {
            var prefix = Guid.NewGuid().ToString("N");
            var userName = prefix;
            var email = prefix + "@test.com";
            provider.CreateUserAndAccount(
                userName, DefaultPassword, true, new Dictionary<string, object> { { "Email", email } });
            return new TestUser(userName, email, DefaultPassword);
        }

        public static TestUser WithUnregisteredUser(this MembershipProvider provider)
        {
            var prefix = Guid.NewGuid().ToString("N");
            var userName = prefix;
            var email = prefix + "@test.com";
            return new TestUser(userName, email, DefaultPassword);
        }
    }
}