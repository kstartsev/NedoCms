using System;
using System.Collections.Generic;
using System.Linq;

namespace NedoCms.Common.Extensions
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}" />
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Converts single element to collection
		/// </summary>
		public static IEnumerable<T> Enumerate<T>(this T item)
		{
			yield return item;
		}

		/// <summary>
		/// Checks if provided collection is null and returns empty collection. Use to avoid NRE
		/// </summary>
		public static IEnumerable<T> Safe<T>(this IEnumerable<T> enumerable)
		{
			return enumerable ?? Enumerable.Empty<T>();
		}

		/// <summary>
		/// Straight forward implementation of FirstOrDefault with explicit default value
		/// </summary>
		public static T FirstOr<T>(this IEnumerable<T> enumerable, T @default)
		{
			using (var enumerator = enumerable.Safe().GetEnumerator())
			{
				if (enumerator.MoveNext()) return enumerator.Current;
			}
			return @default;
		}

		/// <summary>
		/// Unites collection only if condition is met
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="head">First collection</param>
		/// <param name="condition">Condition to check</param>
		/// <param name="tail">Condition to append</param>
		/// <returns>If condition is met, united collection is returned, otherwise, only original collection is returns</returns>
		public static IEnumerable<T> UnionIf<T>(this IEnumerable<T> head, Func<bool> condition, Func<IEnumerable<T>> tail)
		{
			return condition()
				       ? head.Union(tail())
				       : head;
		}

		/// <summary>
		/// Unites collections if first one is empty
		/// </summary>
		public static IEnumerable<T> UnionIfEmpty<T>(this IEnumerable<T> first, Func<IEnumerable<T>> second)
		{
			var f = first.Safe().ToArray();

			return f.UnionIf(() => !f.Any(), second);
		}
	}
}