namespace BetterMembership.IntegrationTests.Helpers
{
    using WebMatrix.WebData;

    internal static class TestUserExtensions
    {
        internal static TestUser WithInvalidPasswordAttempts(this TestUser testUser, int attempts)
        {
            for (var i = 0; i < attempts; i++)
            {
                WebSecurity.Login(testUser.UserName, "invalidPassword");
            }

            return testUser;
        }
    }
}