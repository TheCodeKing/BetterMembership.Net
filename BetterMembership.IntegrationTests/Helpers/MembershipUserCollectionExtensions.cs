namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web.Security;

    internal static class MembershipUserCollectionExtensions
    {
        public static MembershipUser[] ToArray(this MembershipUserCollection collection)
        {
            var users = new MembershipUser[collection.Count];
            collection.CopyTo(users, 0);
            return users;
        }

        public static MembershipUser First(this MembershipUserCollection collection)
        {
            return ToArray(collection)[0];
        }
    }
}