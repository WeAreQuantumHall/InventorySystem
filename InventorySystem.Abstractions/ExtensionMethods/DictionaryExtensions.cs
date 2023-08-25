using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySystem.Abstractions.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<TValue> WhereValue<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            Func<TValue, bool> predicate)
            =>
                dictionary.Values
                    .Where(v => v != null)
                    .Where(predicate);
    }
}