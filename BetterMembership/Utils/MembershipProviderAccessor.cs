namespace BetterMembership.Web
{
    using System;
    using System.Reflection;
    using System.Web.Security;

    internal class MembershipProviderAccessor
    {
        private const string InitializedBackingFieldName = "s_Initialized";

        private const string InitializedDefaultProviderBackingFieldName = "s_InitializedDefaultProvider";

        private const string ProviderBackingFieldName = "s_Provider";

        private static readonly Func<bool, bool> InitializedDefaultProviderSetter;

        private static readonly Func<bool, bool> InitializedSetter;

        private static readonly Func<MembershipProvider, MembershipProvider> ProviderSetter;

        static MembershipProviderAccessor()
        {
            var type = typeof(Membership);
            var providerFieldInfo = type.GetField(
                ProviderBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (providerFieldInfo != null)
            {
                ProviderSetter = provider =>
                    {
                        var value = providerFieldInfo.GetValue(null) as MembershipProvider;
                        providerFieldInfo.SetValue(null, provider);
                        return value;
                    };
            }

            var initializedFieldInfo = type.GetField(
                InitializedBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (initializedFieldInfo != null)
            {
                InitializedSetter = v =>
                    {
                        var value = (bool)initializedFieldInfo.GetValue(null);
                        if (value != v)
                        {
                            initializedFieldInfo.SetValue(null, v);
                        }

                        return value;
                    };
            }

            var initializedDefaultProviderFieldInfo = type.GetField(
                InitializedDefaultProviderBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (initializedDefaultProviderFieldInfo != null)
            {
                InitializedDefaultProviderSetter = v =>
                    {
                        var value = (bool)initializedDefaultProviderFieldInfo.GetValue(null);
                        if (value != v)
                        {
                            initializedDefaultProviderFieldInfo.SetValue(null, v);
                        }

                        return value;
                    };
            }
        }

        public bool SetInitializedAndReturnOriginalValue(bool value)
        {
            return InitializedSetter(value);
        }

        public bool SetInitializedDefaultProviderAndReturnOriginalValue(bool value)
        {
            return InitializedDefaultProviderSetter(value);
        }

        public MembershipProvider SetProviderAndReturnOriginalValue(MembershipProvider provider)
        {
            return ProviderSetter(provider);
        }
    }
}