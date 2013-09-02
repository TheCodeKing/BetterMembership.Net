namespace BetterMembership.Website.Models
{
    using System.Data.Entity;

    public class UsersContext : DbContext
    {
        public UsersContext()
            : this("DefaultConnection")
        {
        }

        public UsersContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}