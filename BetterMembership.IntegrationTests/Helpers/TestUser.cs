namespace BetterMembership.IntegrationTests.Helpers
{
    internal class TestUser
    {
        public TestUser(string userName, string email, string password)
        {
            this.UserName = userName;
            this.Email = email;
            this.Password = password;
        }

        public string Email { get; private set; }

        public string Password { get; private set; }

        public string UserName { get; private set; }
    }
}