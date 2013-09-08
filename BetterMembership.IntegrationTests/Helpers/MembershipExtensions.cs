namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;

    using BetterMembership.Web;

    internal static class MembershipExtensions
    {
        private const string DefaultPassword = "Password1!";

        public static BetterMembershipProvider AsBetter(this MembershipProvider provider)
        {
            return provider as BetterMembershipProvider;
        }

        public static bool HasEmailColumnDefined(this MembershipProvider provider)
        {
            return provider.AsBetter().HasEmailColumnDefined;
        }

        public static int PasswordLockoutTimeoutInSeconds(this MembershipProvider provider)
        {
            return provider.AsBetter().PasswordLockoutTimeoutInSeconds;
        }

        public static FluentProvider<TestUser> WithConfirmedUser(this MembershipProvider provider)
        {
            var prefix = Guid.NewGuid().ToString("N");
            var profile = new Dictionary<string, object>();
            var userName = prefix;
            var email = prefix + "@test.com";
            if (provider.HasEmailColumnDefined())
            {
                profile.Add(GetEmailColumn(provider.AsBetter()), email);
            }
            provider.AsBetter()
                    .CreateUserAndAccount(
                        userName, 
                        DefaultPassword, 
                       profile);
            return new FluentProvider<TestUser>(provider, new TestUser(userName, email, DefaultPassword));
        }

        public static FluentProvider<IList<TestUser>> WithConfirmedUsers(
            this MembershipProvider provider, int count, out string prefix)
        {
            prefix = Guid.NewGuid().ToString("N");
            var users = new List<TestUser>();
            for (var i = 0; i < count; i++)
            {
                var profile = new Dictionary<string, object>();
                var userName = prefix + "_" + i;
                var email = prefix + i + "@test.com";
                if (provider.HasEmailColumnDefined())
                {
                    profile.Add(GetEmailColumn(provider.AsBetter()), email);
                }

                provider.AsBetter()
                        .CreateUserAndAccount(
                            userName, 
                            DefaultPassword,
                            profile);
                users.Add(new TestUser(userName, email, DefaultPassword));
            }

            return new FluentProvider<IList<TestUser>>(provider, users);
        }

        public static FluentProvider<TestUser> WithPasswordLockout(this FluentProvider<TestUser> provider)
        {
            for (var i = 0; i < provider.Provider.AsBetter().MaxInvalidPasswordAttempts + 1; i++)
            {
                provider.Provider.ValidateUser(provider.Value.UserName, "WrongPassword");
            }

            return provider;
        }

        public static FluentProvider<TestUser> WithUnconfirmedUser(this MembershipProvider provider)
        {
            var profile = new Dictionary<string, object>();
            var prefix = Guid.NewGuid().ToString("N");
            var userName = prefix;
            var email = prefix + "@test.com";
            if (provider.HasEmailColumnDefined())
            {
                profile.Add(GetEmailColumn(provider.AsBetter()), email);
            }

            provider.AsBetter()
                    .CreateUserAndAccount(
                        userName, 
                        DefaultPassword, 
                        true,
                        profile);
            return new FluentProvider<TestUser>(provider, new TestUser(userName, email, DefaultPassword));
        }

        public static FluentProvider<TestUser> WithUnregisteredUser(
            this MembershipProvider provider, string userName = null, string email = null)
        {
            var userNameParam = userName ?? Guid.NewGuid().ToString("N");
            var emailParam = email ?? userNameParam + "@test.com";
            return new FluentProvider<TestUser>(provider, new TestUser(userNameParam, emailParam, DefaultPassword));
        }

        private static string GetEmailColumn(BetterMembershipProvider provider)
        {
            return provider.AsBetter().UserEmailColumn ?? "Email";
        }
    }
}