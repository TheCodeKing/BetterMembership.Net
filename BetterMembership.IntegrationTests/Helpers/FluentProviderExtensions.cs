namespace BetterMembership.IntegrationTests.Helpers
{
    internal static class FluentProviderExtensions
    {
        internal static FluentProvider<TestUser> WithInvalidPasswordAttempts(
            this FluentProvider<TestUser> fluent, int attempts)
        {
            for (var i = 0; i < attempts; i++)
            {
                fluent.Provider.ValidateUser(fluent.Value.UserName, "invalidPassword");
            }

            return fluent;
        }
    }
}