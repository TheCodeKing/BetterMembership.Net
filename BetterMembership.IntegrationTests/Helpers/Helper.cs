namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Security;

    using BetterMembership.Data;

    using WebMatrix.WebData;

    internal static class MembershipUserCollectionExtensions
    {
        public static MembershipUser[] ToArray(this MembershipUserCollection collection)
        {
            var users = new MembershipUser[collection.Count];
            collection.CopyTo(users, 0);
            return users;
        }
    }

    internal static class Helper
    {
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

        public static IList<TestUser> WithUsers(int count, out string prefix)
        {
            prefix = Guid.NewGuid().ToString("N");
            var users = new List<TestUser>();
            for (var i = 0; i < count; i++)
            {
                var userName = prefix + "_" + i;
                var email = prefix + i + "@test.com";
                const string Password = "Password1!";
                WebSecurity.CreateUserAndAccount(userName, Password, new {Email = email}, true);
                users.Add(new TestUser(userName, email));
            }

            return users;
        }

        public static TestUser WithUser(out string prefix)
        {
            prefix = Guid.NewGuid().ToString("N");
            var i = 0;
            var userName = prefix + "_" + i;
            var email = prefix + i + "@test.com";
            const string Password = "Password1!";
            WebSecurity.CreateUserAndAccount(userName, Password, new { Email = email }, true);
            return new TestUser(userName, email);
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

        public class TestUser
        {
            public TestUser(string userName, string email)
            {
                this.UserName = userName;
                this.Email = email;
            }

            public string Email { get; private set; }

            public string UserName { get; private set; }
        }
    }
}