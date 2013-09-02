namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using BetterMembership.Data;

    internal static class Helper
    {
        public static void ClearDownDatabaseTables()
        {
            InitializeDatabase("SqlServerCE");
            InitializeDatabase("SqlServerCE2");
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

        private static void InitializeDatabase(string connectionString)
        {
            using (var context = new MembershipContext(connectionString))
            {
                context.Database.CreateIfNotExists();
            }
        }
    }
}