using System;
using System.Collections.Generic;

namespace NedoCms.Common.Extensions
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Adds or updates value for specfied key
		/// </summary>
		public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
		                                               Func<TKey, TValue> addValueFactory,
		                                               Func<TKey, TValue, TValue> updateValueFactory)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			// TODO: consider checking for ConcurrentDictionary<TKey, TValue>
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = updateValueFactory(key, dictionary[key]);
			}
			else
			{
				dictionary[key] = addValueFactory(key);
			}

			return dictionary[key];
		}

		/// <summary>
		/// Gets value from dictionary. If value with specfied key not found, new value will be created, added to dictionary and returned from method
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			TValue value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = factory(key);
				dictionary.Add(key, value);
			}
			return value;
		}
	}
}