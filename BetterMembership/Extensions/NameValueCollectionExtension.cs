namespace BetterMembership.Extensions
{
    using System.Collections.Specialized;
    using System.Linq;

    using CuttingEdge.Conditions;

    internal static class NameValueCollectionExtension
    {
        public static bool ContainsKey(this NameValueCollection collection, string key)
        {
            if (collection == null)
            {
                return false;
            }

            if (collection.Get(key) == null)
            {
                return collection.AllKeys.Contains(key);
            }

            return true;
        }

        public static bool GetBoolean(this NameValueCollection collection, string key, bool defaultValue = false)
        {
            Condition.Requires(key, "key").IsNotNullOrWhiteSpace();

            if (collection == null)
            {
                return defaultValue;
            }

            var value = collection.GetString(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            bool returnValue;
            return bool.TryParse(value, out returnValue) ? returnValue : defaultValue;
        }

        public static int GetInteger(this NameValueCollection collection, string key, int defaultValue = 0)
        {
            Condition.Requires(key, "key").IsNotNullOrWhiteSpace();

            if (collection == null)
            {
                return defaultValue;
            }

            var value = collection.GetString(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            int returnValue;
            return int.TryParse(value, out returnValue) ? returnValue : defaultValue;
        }

        public static string GetString(this NameValueCollection collection, string key, string defaultValue = null)
        {
            Condition.Requires(key, "key").IsNotNullOrWhiteSpace();

            if (collection == null)
            {
                return defaultValue;
            }

            return collection.Get(key) ?? defaultValue;
        }
    }
}