namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using BetterMembership.Data;

    internal static class HttpContextExtensions
    {
        public static HttpContext WithCleanDatabase(this HttpContext context)
        {
            InitializeDatabase("SqlServer", "UserProfile");
            InitializeDatabase("SqlServerCE2", "CustomUserTable");

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