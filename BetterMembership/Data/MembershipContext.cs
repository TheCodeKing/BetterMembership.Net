namespace BetterMembership.Data
{
    using System.Data.Entity;

    internal class MembershipContext : DbContext
    {
        public MembershipContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}