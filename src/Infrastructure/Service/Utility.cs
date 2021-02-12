using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessagingService.Infrastructure
{
    public static class Utility
    {
        public static bool ProcessorExecuter<ProcessContext, ReturnObject>(this List<Action<ProcessContext, ProcessResult<ReturnObject>>> processors, ProcessContext context, ProcessResult<ReturnObject> processed)
        {
            if (!processed.IsSuccessful)
                return false;

            foreach (var processor in processors)
            {
                processor(context, processed);

                if (!processed.IsSuccessful)
                    return false;
            }
            return true;
        }

        public static bool ProcessorExecuter<ProcessContext, ReturnObject>(ProcessContext context, ProcessResult<ReturnObject> processed, Action<ProcessResult<ReturnObject>> processCalback,
            params Action<ProcessContext, ProcessResult<ReturnObject>>[] processors)
        {
            if (!processed.IsSuccessful)
                return false;

            foreach (var processor in processors)
            {
                processor(context, processed);

                if (!processed.IsSuccessful)
                    return false;
            }
            return true;
        }

        public async static Task<bool> ProcessorExecuter<ProcessContext, ReturnObject>(ProcessContext context, ProcessResult<ReturnObject> processed, Func<ProcessContext, ProcessResult<ReturnObject>, Task> processCalback,
            params Action<ProcessContext, ProcessResult<ReturnObject>>[] processors)
        {
            if (!processed.IsSuccessful)
                return false;

            foreach (var processor in processors)
            {
                processor(context, processed);

                if (!processed.IsSuccessful)
                    return false;
            }

            await processCalback(context, processed);

            return true;
        }

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