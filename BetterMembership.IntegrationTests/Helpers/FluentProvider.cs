namespace BetterMembership.IntegrationTests.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Web.Security;

    using CuttingEdge.Conditions;

    internal class FluentProvider<T>
    {
        private readonly Action<T> lazyCreate;

        private readonly MembershipProvider provider;

        private readonly T value;

        private bool created;

        public FluentProvider(MembershipProvider provider, T value, Action<T> lazyCreate)
        {
            Condition.Requires(provider, "provider").IsNotNull();

            this.provider = provider;
            this.value = value;
            this.lazyCreate = lazyCreate;
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
                if (!this.created)
                {
                    this.created = true;
                    this.lazyCreate(this.value);
                }

                return this.value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal T UpdateValue
        {
            get
            {
                return this.value;
            }
        }
    }
}