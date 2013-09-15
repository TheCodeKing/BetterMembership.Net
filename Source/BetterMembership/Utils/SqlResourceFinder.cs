namespace BetterMembership.Utils
{
    using System.IO;

    using BetterMembership.Facades;

    using CuttingEdge.Conditions;

    internal class SqlResourceFinder
    {
        private readonly IResourceManifestFacade resourceManifestFacade;

        public SqlResourceFinder(IResourceManifestFacade resourceManifestFacade)
        {
            Condition.Requires(resourceManifestFacade, "resourceManifestFacade").IsNotNull();

            this.resourceManifestFacade = resourceManifestFacade;
        }

        public string LocateScript(string sqlQueryName, string sqlProvider)
        {
            Condition.Requires(sqlQueryName, "sqlQueryName").IsNotNullOrWhiteSpace();
            Condition.Requires(sqlProvider, "sqlProvider").IsNotNullOrWhiteSpace();

            var key = GetKeyFromProvider(sqlProvider);

            var primaryResource = string.Concat("BetterMembership.Scripts.", key, ".", sqlQueryName, ".sql");
            var defaultResource = string.Concat("BetterMembership.Scripts.Default.", sqlQueryName, ".sql");

            string script;
            if (this.TryGetResource(primaryResource, out script))
            {
                return script;
            }

            if (this.TryGetResource(defaultResource, out script))
            {
                return script;
            }

            throw new FileNotFoundException(
                string.Format("{0} sql script not found for provider {1}", sqlQueryName, sqlProvider));
        }

        private static string GetKeyFromProvider(string sqlProvider)
        {
            if (sqlProvider.Contains("SqlServerCe"))
            {
                return "SqlServerCe";
            }

            return "Default";
        }

        private bool TryGetResource(string primaryResource, out string script)
        {
            try
            {
                using (var stream = this.resourceManifestFacade.GetManifestResourceStream(primaryResource))
                {
                    if (stream == null)
                    {
                        script = null;
                        return false;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        script = reader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                script = null;
                return false;
            }
        }
    }
}