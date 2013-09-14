namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Collections.Generic;

    internal class TestUser
    {
        private readonly Dictionary<string, object> profile;

        public TestUser(string userName, string email, string password, Dictionary<string, object> profile)
        {
            this.profile = profile;
            this.UserName = userName;
            this.Email = email;
            this.Password = password;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public Dictionary<string, object> Profile
        {
            get
            {
                return this.profile;
            }
        }

        public string UserName { get; set; }
    }
}