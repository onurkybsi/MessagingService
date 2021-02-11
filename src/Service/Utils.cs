using System.Collections.Generic;

namespace MessagingService.Service
{
    public static class Utils
    {
        public static void AddIfNoExist<TValue>(this Dictionary<string, TValue> dictionary, string key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static void RemoveIfExist<TValue>(this Dictionary<string, TValue> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }

        public static void Upsert<TValue>(this Dictionary<string, TValue> dictionary, string key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddIfNoExist(this HashSet<string> hashSet, string value)
        {
            if (!hashSet.Contains(value))
            {
                hashSet.Add(value);
            }
        }

        public static void RemoveIfExist(this HashSet<string> hashSet, string value)
        {
            if (hashSet.Contains(value))
            {
                hashSet.Remove(value);
            }
        }
    }
}