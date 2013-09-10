namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using BetterMembership.Data;

    internal static class HttpContextExtensions
    {
        public static HttpContext WithCleanDatabase(this HttpContext context)
        {
            InitializeDatabase("SqlServer", "UserProfile");
            InitializeDatabase("SqlServer", "CustomUserTable");
            InitializeDatabase("SqlServerCe1", "UserProfile");
            InitializeDatabase("SqlServerCe2", "CustomUserTable");
            InitializeDatabase("SqlServerCe3", "UserProfile");

            return context;
        }

        public static HttpContext WithHttpContext(this HttpContext context)
        {
            if (context != null)
            {
                return context;
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

            return HttpContext.Current;
        }

        private static void InitializeDatabase(string connectionString, string userTable)
        {
            using (var context = new MembershipContext(connectionString))
            {
                context.Database.CreateIfNotExists();


                if (
                    context.Database.SqlQuery<int?>(
                        "SELECT 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'webpages_UsersInRoles'")
                           .SingleOrDefault() != null)
                {
                    context.Database.ExecuteSqlCommand("Delete From webpages_UsersInRoles");
                    context.Database.ExecuteSqlCommand("Delete From webpages_Roles");
                }

                if (
                    context.Database.SqlQuery<int?>(
                        "SELECT 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'webpages_Membership'")
                           .SingleOrDefault() != null)
                {

                    context.Database.ExecuteSqlCommand("Delete From webpages_Membership");
                }

                if (
                    context.Database.SqlQuery<int?>(
                        "SELECT 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = {0}", userTable).SingleOrDefault()
                    != null)
                {
                    context.Database.ExecuteSqlCommand("Delete From " + userTable);
                }
            }
        }
    }
}