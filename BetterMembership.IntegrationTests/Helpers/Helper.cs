namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.Data;

    using WebMatrix.WebData;

    internal static class Helper
    {
        private const string DefaultPassword = "Password1!";

        public static void ClearDownDatabaseTables()
        {
            InitializeDatabase("SqlServer", "UserProfile");
            InitializeDatabase("SqlServerCE2", "CustomUserTable");
        }

        public static void SetupCurrentHttpContext(this HttpContext context)
        {
            if (context != null)
            {
                return;
            }

            var capabilities = new Dictionary<string, string> { { "cookies", "true" } };
            var request = new HttpRequest("index.html", "http://foo", string.Empty)
                              {
                                  Browser =
                                      new HttpBrowserCapabilities
                                          {
                                              Capabilities
                                                  =
                                                  capabilities
                                          }
                              };
            HttpContext.Current = new HttpContext(request, new HttpResponse(new StringWriter()));
        }

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

        private static void InitializeDatabase(string connectionString, string userTable)
        {
            using (var context = new MembershipContext(connectionString))
            {
                context.Database.CreateIfNotExists();
                try
                {
                    context.Database.ExecuteSqlCommand("truncate table " + userTable);
                }
                catch (Exception)
                {
                }

                try
                {
                    context.Database.ExecuteSqlCommand("truncate table webpages_Membership");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}