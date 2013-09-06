namespace BetterMembership.Utils
{
    using System;
    using System.Reflection;
    using System.Web.Security;

    internal class RoleProviderAccessor
    {
        private const string EnabledBackingFieldName = "s_Enabled";

        private const string InitializedBackingFieldName = "s_Initialized";

        private const string InitializedDefaultProviderBackingFieldName = "s_InitializedDefaultProvider";

        private const string ProviderBackingFieldName = "s_Provider";

        private const string ProvidersBackingFieldName = "s_Providers";

        private static readonly Func<bool, bool> EnabledSetter;

        private static readonly Func<bool, bool> InitializedDefaultProviderSetter;

        private static readonly Func<bool, bool> InitializedSetter;

        private static readonly Func<RoleProvider, RoleProvider> ProviderSetter;

        private static readonly Func<RoleProviderCollection, RoleProviderCollection> ProvidersSetter;

        static RoleProviderAccessor()
        {
            var type = typeof(Roles);
            var providerFieldInfo = type.GetField(
                ProviderBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (providerFieldInfo != null)
            {
                ProviderSetter = provider =>
                    {
                        var value = providerFieldInfo.GetValue(null) as RoleProvider;
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

            var enabledFieldInfo = type.GetField(EnabledBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (enabledFieldInfo != null)
            {
                EnabledSetter = v =>
                    {
                        var value = (bool)enabledFieldInfo.GetValue(null);
                        if (value != v)
                        {
                            enabledFieldInfo.SetValue(null, v);
                        }

                        return value;
                    };
            }

            var providersFieldInfo = type.GetField(
                ProvidersBackingFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (providersFieldInfo != null)
            {
                ProvidersSetter = v =>
                    {
                        var value = (RoleProviderCollection)providersFieldInfo.GetValue(null);
                        if (value != v)
                        {
                            providersFieldInfo.SetValue(null, v);
                        }

                        return value;
                    };
            }
        }

        public bool SetEnabledAndReturnOriginalValue(bool value)
        {
            return EnabledSetter(value);
        }

        public bool SetInitializedAndReturnOriginalValue(bool value)
        {
            return InitializedSetter(value);
        }

        public bool SetInitializedDefaultProviderAndReturnOriginalValue(bool value)
        {
            return InitializedDefaultProviderSetter(value);
        }

        public RoleProvider SetProviderAndReturnOriginalValue(RoleProvider provider)
        {
            return ProviderSetter(provider);
        }

        public RoleProviderCollection SetProvidersAndReturnOriginalValue(RoleProviderCollection providers)
        {
            return ProvidersSetter(providers);
        }
    }
}