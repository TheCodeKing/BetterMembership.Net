namespace BetterMembership.Web
{
    using System;

    using WebMatrix.WebData;

    internal static class ExtendedMembershipProviderExtensions
    {
        public static bool IsAccountLockedOut(
            this ExtendedMembershipProvider provider, string userName, int allowedPasswordAttempts, TimeSpan interval)
        {
            return provider.GetUser(userName, false) != null
                   && provider.GetPasswordFailuresSinceLastSuccess(userName) > allowedPasswordAttempts
                   && provider.GetLastPasswordFailureDate(userName).Add(interval) > DateTime.UtcNow;
        }

        public static bool IsAccountLockedOut(
            this ExtendedMembershipProvider provider, string userName, int allowedPasswordAttempts, int intervalSeconds)
        {
            return IsAccountLockedOut(
                provider, userName, allowedPasswordAttempts, TimeSpan.FromSeconds(intervalSeconds));
        }
    }
}