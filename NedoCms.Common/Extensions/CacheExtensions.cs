using System;
using System.Web.Caching;

namespace NedoCms.Common.Extensions
{
	/// <summary>
	/// Extensions for <see cref="Cache"/> class
	/// </summary>
	public static class CacheExtensions
	{
		/// <summary>
		/// Gets value from cache.If value with specfied key not found, new value will be created, added to cache and returned from method
		/// </summary>
		public static TValue GetOrAdd<TValue>(this Cache cache, string key, Func<string, TValue> factory) where TValue : class
		{
			if (cache == null) throw new ArgumentNullException("cache");

			var val = cache[key] as TValue;
			if (val == null)
			{
				val = factory(key);
				cache[key] = val;
			}
			return val;
		}
	}
}