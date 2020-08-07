using System;
using System.Collections.Generic;

namespace Cogworks.AzureSearch.ConsoleApp.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            IDictionary<TKey, TValue> collection,
            bool @override = true)
        {
            if (collection == null)
            {
                throw new ArgumentNullException($"{nameof(collection)}", "Collection is not set.");
            }

            foreach (var (key, value) in collection)
            {
                if (!source.ContainsKey(key))
                {
                    source.Add(key, value);
                }
                else
                {
                    if (@override)
                    {
                        source[key] = value;
                    }
                }
            }
        }

        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> source,
            out TKey key,
            out TValue value)
        {
            key = source.Key;
            value = source.Value;
        }
    }
}