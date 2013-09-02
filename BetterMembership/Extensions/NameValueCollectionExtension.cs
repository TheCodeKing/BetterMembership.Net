namespace BetterMembership.Extensions
{
    using System.Collections.Specialized;

    using CuttingEdge.Conditions;

    internal static class NameValueCollectionExtension
    {
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
            if (bool.TryParse(value, out returnValue))
            {
                return returnValue;
            }

            return defaultValue;
        }

        public static int GetInteger(this NameValueCollection collection, string key, int defaultValue = 0)
        {
            Condition.Requires(key, "key").IsNotNullOrWhiteSpace();

            if (collection == null)
            {
                return 0;
            }

            var value = collection.GetString(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            int returnValue;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        public static string GetString(this NameValueCollection collection, string key, string defaultValue = null)
        {
            Condition.Requires(key, "key").IsNotNullOrWhiteSpace();

            if (collection == null)
            {
                return null;
            }

            return collection.Get(key) ?? defaultValue;
        }
    }
}