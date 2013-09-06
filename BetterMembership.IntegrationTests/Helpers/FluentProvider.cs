namespace BetterMembership.IntegrationTests.Helpers
{
    using System.Web.Security;

    using CuttingEdge.Conditions;

    internal class FluentProvider<T>
    {
        private readonly MembershipProvider provider;

        private readonly T value;

        public FluentProvider(MembershipProvider provider, T value)
        {
            Condition.Requires(provider, "provider").IsNotNull();

            this.provider = provider;
            this.value = value;
        }

        public MembershipProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        public T Value
        {
            get
            {
                return this.value;
            }
        }
    }
}