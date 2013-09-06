namespace BetterMembership.Utils
{
    using System;
    using System.Web.Security;

    using BetterMembership.Dummy;

    using CuttingEdge.Conditions;

    internal sealed class DefaultProviderSwitcher : IDisposable
    {
        private readonly MembershipProviderAccessor membershipProviderAccessor;

        private readonly bool originalMembershipInitialized;

        private readonly bool originalMembershipInitializedDefault;

        private readonly MembershipProvider originalMembershipProvider;

        private readonly bool originalRoleEnabledInitialized;

        private readonly bool originalRoleInitialized;

        private readonly bool originalRoleInitializedDefault;

        private readonly RoleProvider originalRoleProvider;

        private readonly RoleProviderCollection originalRoleProviders;

        private readonly RoleProviderAccessor roleProviderAccessor;

        public DefaultProviderSwitcher(RoleProvider provider)
            : this(DummyMembershipProvider.Default, provider)
        {
        }

        public DefaultProviderSwitcher(MembershipProvider provider)
            : this(provider, DummyRoleProvider.Default)
        {
        }

        public DefaultProviderSwitcher(MembershipProvider provider, RoleProvider roleProvider)
            : this(new MembershipProviderAccessor(), new RoleProviderAccessor(), provider, roleProvider)
        {
        }

        public DefaultProviderSwitcher(
            MembershipProviderAccessor membershipProviderAccessor, 
            RoleProviderAccessor roleProviderAccessor, 
            MembershipProvider provider, 
            RoleProvider roleProvider)
        {
            Condition.Requires(membershipProviderAccessor, "membershipProviderAccessor").IsNotNull();
            Condition.Requires(roleProviderAccessor, "roleProviderAccessor").IsNotNull();
            Condition.Requires(provider, "provider").IsNotNull();
            Condition.Requires(roleProvider, "roleProvider").IsNotNull();

            this.membershipProviderAccessor = membershipProviderAccessor;
            this.roleProviderAccessor = roleProviderAccessor;

            this.originalMembershipInitializedDefault =
                membershipProviderAccessor.SetInitializedDefaultProviderAndReturnOriginalValue(true);
            this.originalMembershipInitialized = membershipProviderAccessor.SetInitializedAndReturnOriginalValue(true);
            this.originalMembershipProvider = membershipProviderAccessor.SetProviderAndReturnOriginalValue(provider);

            this.originalRoleEnabledInitialized = roleProviderAccessor.SetEnabledAndReturnOriginalValue(true);
            this.originalRoleProviders =
                roleProviderAccessor.SetProvidersAndReturnOriginalValue(new RoleProviderCollection());
            this.originalRoleInitializedDefault =
                roleProviderAccessor.SetInitializedDefaultProviderAndReturnOriginalValue(true);
            this.originalRoleInitialized = roleProviderAccessor.SetInitializedAndReturnOriginalValue(true);
            this.originalRoleProvider = roleProviderAccessor.SetProviderAndReturnOriginalValue(roleProvider);
        }

        ~DefaultProviderSwitcher()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.membershipProviderAccessor.SetInitializedAndReturnOriginalValue(this.originalMembershipInitialized);
                this.membershipProviderAccessor.SetInitializedDefaultProviderAndReturnOriginalValue(
                    this.originalMembershipInitializedDefault);
                this.membershipProviderAccessor.SetProviderAndReturnOriginalValue(this.originalMembershipProvider);

                this.roleProviderAccessor.SetInitializedAndReturnOriginalValue(this.originalRoleInitialized);
                this.roleProviderAccessor.SetInitializedDefaultProviderAndReturnOriginalValue(
                    this.originalRoleInitializedDefault);
                this.roleProviderAccessor.SetProviderAndReturnOriginalValue(this.originalRoleProvider);
                this.roleProviderAccessor.SetEnabledAndReturnOriginalValue(this.originalRoleEnabledInitialized);
                this.roleProviderAccessor.SetProvidersAndReturnOriginalValue(this.originalRoleProviders);
            }
        }
    }
}