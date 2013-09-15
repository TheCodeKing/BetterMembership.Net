namespace BetterMembership.Facades
{
    using System.IO;
    using System.Reflection;

    using CuttingEdge.Conditions;

    internal class ResourceManifestFacade : IResourceManifestFacade
    {
        private readonly Assembly assembly;

        internal ResourceManifestFacade()
            : this(typeof(ResourceManifestFacade).Assembly)
        {
        }

        public ResourceManifestFacade(Assembly assembly)
        {
            Condition.Requires(assembly, "assembly").IsNotNull();

            this.assembly = assembly;
        }

        public virtual Stream GetManifestResourceStream(string resource)
        {
            Condition.Requires(resource, "resource").IsNotNullOrWhiteSpace();

            return assembly.GetManifestResourceStream(resource);
        }
    }
}