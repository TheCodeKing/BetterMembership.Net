namespace BetterMembership.Facades
{
    using System.IO;

    internal interface IResourceManifestFacade
    {
        Stream GetManifestResourceStream(string resource);
    }
}