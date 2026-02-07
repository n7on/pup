using System.Collections;
using System.Collections.Generic;

namespace Pup.Common
{
    public static class PowerShellHelper
    {
        /// <summary>
        /// Converts a PowerShell Hashtable to a Dictionary, recursively converting nested structures.
        /// </summary>
        public static Dictionary<string, object> ConvertHashtable(Hashtable ht)
        {
            if (ht == null)
                return null;

            var result = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in ht)
            {
                var key = entry.Key?.ToString();
                if (key != null)
                {
                    result[key] = ConvertValue(entry.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts an IDictionary to a Dictionary, recursively converting nested structures.
        /// </summary>
        public static Dictionary<string, object> ConvertDictionary(IDictionary dict)
        {
            if (dict == null)
                return null;

            var result = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in dict)
            {
                var key = entry.Key?.ToString();
                if (key != null)
                {
                    result[key] = ConvertValue(entry.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts an object (Hashtable, IDictionary, or other) to a format suitable for serialization.
        /// Recursively converts nested Hashtables and collections.
        /// </summary>
        public static object ConvertValue(object value)
        {
            if (value == null)
                return null;

            if (value is Hashtable ht)
                return ConvertHashtable(ht);

            if (value is IDictionary dict)
                return ConvertDictionary(dict);

            if (value is IList list && !(value is string))
            {
                var result = new List<object>();
                foreach (var item in list)
                {
                    result.Add(ConvertValue(item));
                }
                return result;
            }

            return value;
        }
    }
}
