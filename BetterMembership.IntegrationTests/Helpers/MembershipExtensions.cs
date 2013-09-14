namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public static int PasswordAttemptWindowInSeconds(this MembershipProvider provider)
        {
            return provider.AsBetter().PasswordAttemptWindowInSeconds;
        }

        public static FluentProvider<TestUser> WithConfirmedUser(this MembershipProvider provider)
        {
            var prefix = Guid.NewGuid().ToString("N");
            var profile = new Dictionary<string, object>();
            var userName = prefix;
            var email = prefix + "@test.com";

            Action<TestUser> lazyCreate = t =>
                {
                    if (provider.HasEmailColumnDefined())
                    {
                        profile.Add(provider.AsBetter().UserEmailColumn, t.Email);
                    }

                    provider.AsBetter().CreateUserAndAccount(t.UserName, t.Password, t.Profile);
                };
            var testUser = new TestUser(userName, email, DefaultPassword, profile);
            return new FluentProvider<TestUser>(provider, testUser, lazyCreate);
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
                var testUser = new TestUser(userName, email, DefaultPassword, profile);
                users.Add(testUser);
            }

            Action<IList<TestUser>> lazyCreate = t => t.ToList().ForEach(u =>
                {
                    if (provider.HasEmailColumnDefined())
                    {
                        u.Profile.Add(provider.AsBetter().UserEmailColumn, u.Email);
                    }

                    provider.AsBetter().CreateUserAndAccount(u.UserName, u.Password, u.Profile);
                });
            return new FluentProvider<IList<TestUser>>(provider, users, lazyCreate);
        }

        public static FluentProvider<TestUser> WithPasswordLength(this FluentProvider<TestUser> provider, int count)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < provider.Provider.MinRequiredNonAlphanumericCharacters; i++)
            {
                builder.Append("!");
            }

            provider.UpdateValue.Password = builder.ToString().PadRight(count, 'x');
            return provider;
        }

        public static FluentProvider<TestUser> WithUserNameLength(this FluentProvider<TestUser> provider, int count)
        {
            provider.UpdateValue.UserName = string.Empty.PadRight(count, 'x');
            return provider;
        }

        public static MembershipUser AsMembershipUser(this FluentProvider<TestUser> provider)
        {
            var testUser = provider.Value;
            return provider.Provider.GetUser(testUser.UserName, false);
        }

        public static FluentProvider<TestUser> WithEmailLength(this FluentProvider<TestUser> provider, int count)
        {
            provider.UpdateValue.Email = "@a.com".PadLeft(count, 'x');
            return provider;
        }

        public static FluentProvider<TestUser> WithPasswordLockout(this FluentProvider<TestUser> provider)
        {
            for (var i = 0; i < provider.Provider.AsBetter().MaxInvalidPasswordAttempts + 1; i++)
            {
                provider.Provider.ValidateUser(provider.Value.UserName, "WrongPassword");
            }

            return provider;
        }

        public static string[] WithUserInRoles(this RoleProvider roleProvider, string userName, params string[] roles)
        {
            roles.ToList().ForEach(
                x =>
                    {
                        if (!roleProvider.RoleExists(x))
                        {
                            roleProvider.CreateRole(x);
                        }

                        roleProvider.AddUsersToRoles(new[] { userName }, new[] { x });
                    });

            return roles;
        }

        public static FluentProvider<TestUser> WithUnconfirmedUser(this MembershipProvider provider)
        {
            var profile = new Dictionary<string, object>();
            var prefix = Guid.NewGuid().ToString("N");
            var userName = prefix;
            var email = prefix + "@test.com";
 
            Action<TestUser> lazyCreate = t =>
                {
                    if (provider.HasEmailColumnDefined())
                    {
                        profile.Add(provider.AsBetter().UserEmailColumn, t.Email);
                    }
                    provider.AsBetter().CreateUserAndAccount(t.UserName, t.Password, true, t.Profile);
                };
            var testUser = new TestUser(userName, email, DefaultPassword, profile);
            return new FluentProvider<TestUser>(provider, testUser, lazyCreate);
        }

        public static FluentProvider<TestUser> WithNonAlphaNumericCharsInPassword(this FluentProvider<TestUser> provider, int chars)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < chars; i++)
            {
                builder.Append("!");
            }

            provider.UpdateValue.Password = builder.ToString().PadRight(provider.Provider.MinRequiredPasswordLength, 'x');
            return provider;
        }

        public static FluentProvider<TestUser> WithUnregisteredUser(
            this MembershipProvider provider, string userName = null, string email = null)
        {
            var userNameParam = userName ?? Guid.NewGuid().ToString("N");
            var emailParam = email ?? userNameParam + "@test.com";
            if (!provider.AsBetter().HasEmailColumnDefined)
            {
                emailParam = null;
            }

            return new FluentProvider<TestUser>(
                provider, new TestUser(userNameParam, emailParam, DefaultPassword, new Dictionary<string, object>()), t => { });
        }
    }
}