using System;
using System.Collections.Generic;

namespace NedoCms.Common.Extensions
{
	public static class FunctionalExtensions
	{
		/// <summary>
		/// Returns it's argument as is.
		/// </summary>
		public static T Id<T>(T t)
		{
			return t;
		}

		/// <summary>
		/// Empty function
		/// </summary>
		public static void DoNothing<T>(T _)
		{
			/*Does nothing.*/
		}

		/// <summary>
		/// Returns new, memoized function, backed by cache.
		/// Memoized function will first look up input value in cache and if found, returns it.
		/// If input value wasn't found in cache, then original function f will be called and result stored in cache and returned.
		/// </summary>
		public static Func<T1, T2> Memoize<T1, T2>(this Func<T1, T2> f, IDictionary<T1, T2> cache)
		{
			if (f == null) throw new ArgumentNullException("f", "Function to memoize is null");
			if (cache == null) throw new ArgumentNullException("cache", "Memoization cache is null");
			if (cache.IsReadOnly) throw new ArgumentException("Memoization cache is read only");

			return input => cache.GetOrAdd(input, f);
		}
	}
}
