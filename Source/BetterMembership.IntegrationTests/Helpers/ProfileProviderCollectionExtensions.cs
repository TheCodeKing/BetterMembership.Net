namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web.Profile;
    using System.Web.Security;

    internal static class ProfileProviderCollectionExtensions
    {
        public static ProfileInfo First(this ProfileInfoCollection collection)
        {
            return ToArray(collection)[0];
        }

        public static ProfileInfo[] ToArray(this ProfileInfoCollection collection)
        {
            var profiles = new ProfileInfo[collection.Count];
            collection.CopyTo(profiles, 0);
            return profiles;
        }
    }
}