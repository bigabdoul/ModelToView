using System;
using System.Collections.Generic;

namespace Carfamsoft.ModelToView.Extensions
{
    /// <summary>
    /// Provides extension methods for instances of the collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>Adds a new attribute or optionally replaces an existing attribute in the destination attributes.</summary>
        /// <param name="attributes">The attributes to wich <paramref name="key"/> and <paramref name="value"/> will be merged.</param>
        /// <param name="key">The key for the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="replaceExisting">true to replace an existing attribute if an attribute exists that has the specified <paramref name="key" /> value, or false to leave the original attribute unchanged.</param>
        public static void MergeAttribute<TKey, TValue>(this IDictionary<TKey, TValue> attributes, TKey key, TValue value, bool replaceExisting = false)
        {
            if (Equals(default(TKey), key) || string.IsNullOrEmpty(key.ToString()))
            {
                throw new ArgumentException("Argument cannot be null or empty", "key");
            }

            if (replaceExisting || !attributes.ContainsKey(key))
            {
                attributes[key] = value;
            }
        }

        /// <summary>Adds new attributes or optionally replaces existing attributes.</summary>
        /// <param name="attributes">The source collection of attributes to merge with the destination attributes.</param>
        /// <param name="destinationAttributes">The target collection of attributes to which to merge.</param>
        /// <param name="replaceExisting">For each attribute in <paramref name="attributes" />, true to replace the attribute if an attribute already exists that has the same key, or false to leave the original attribute unchanged.</param>
        /// <typeparam name="TKey">The type of the key object.</typeparam>
        /// <typeparam name="TValue">The type of the value object.</typeparam>
        public static IDictionary<TKey, TValue> MergeAttributes<TKey, TValue>(this IDictionary<TKey, TValue> attributes, IDictionary<TKey, TValue> destinationAttributes, bool replaceExisting = false)
        {
            if (attributes != null)
            {
                foreach (KeyValuePair<TKey, TValue> attribute in attributes)
                {
                    destinationAttributes.MergeAttribute(attribute.Key, attribute.Value, replaceExisting);
                }
            }
            return destinationAttributes;
        }
    }
}
