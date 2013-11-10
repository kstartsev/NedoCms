using System;
using System.Collections.Generic;
using System.Linq;

namespace NedoCms.Common.Extensions
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Eagerly(!) splits `source` collection in two. 
		/// Returns collection of elements satisfying predicate as result.
		/// Non-satisfying elements stored in leftovers. 
		/// </summary>
		public static IEnumerable<T> SplitByPredicate<T>(this IEnumerable<T> source, Func<T, bool> predicate, out IEnumerable<T> leftovers)
		{
			if (null == source) throw new ArgumentNullException("source");
			if (null == predicate) throw new ArgumentNullException("predicate");

			var satisfying = new List<T>();
			var nonSatisfying = new List<T>();
			foreach (var el in source)
			{
				var targetCollection = predicate(el) ? satisfying : nonSatisfying;
				targetCollection.Add(el);
			}
			leftovers = nonSatisfying;
			return satisfying;
		}

		/// <summary>
		/// EAGERLY applies action on each element of source
		/// </summary>
		public static void ForEachDo<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (null == source) throw new ArgumentNullException("source");
			if (null == action) throw new ArgumentNullException("action");

			foreach (var item in source)
			{
				action(item);
			}
		}


		/// <summary>
		/// LAZILY applies action on each element of source. Returns duplicate of source.
		/// </summary>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (null == source) throw new ArgumentNullException("source");
			if (null == action) throw new ArgumentNullException("action");

			foreach (var item in source)
			{
				action(item);
				yield return item;
			}
		}

		/// <summary>
		/// Break a list of items into chunks of a specific size
		/// </summary>
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
		{
			while (source.Any())
			{
				yield return source.Take(chunksize);
				source = source.Skip(chunksize);
			}
		}


		/// <summary>
		/// Alias for "Select" method
		/// </summary>
		public static IEnumerable<T2> Fmap<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> f)
		{
			return source.Select(f);
		}

		/// <summary>
		/// Creates *infinite* stream of initialElement, f(initialElement), f(f(initialElement)), ...
		/// </summary>
		public static IEnumerable<T> Stream<T>(this T initialElement, Func<T, T> f)
		{
			yield return initialElement;

			var element = initialElement;
			while (true)
			{
				element = f(element);
				yield return element;
			}
		}

		/// <summary>
		/// Returns collection based on given one and another one, meeting condition
		/// </summary>
		public static IEnumerable<T> AddIf<T>(this IEnumerable<T> head, Func<bool> condition, Func<IEnumerable<T>> tail)
		{
			foreach (var item in head.Safe()) yield return item;

			if (condition())
			{
				foreach (var item in tail().Safe()) yield return item;
			}
		}

		/// <summary>
		/// Gets collection of unique items by specific parameter
		/// </summary>
		public static IEnumerable<T> DistinctByField<T, TField>(this IEnumerable<T> source, Func<T, TField> fieldDataExtractor)
		{
			return source
				.Zip(source.Select(fieldDataExtractor), Tuple.Create)
				.Distinct(new TupleEqualityComparer<T, TField>())
				.Select(t => t.Item1);
		}

		public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(this IEnumerable<T1> coll1, IEnumerable<T2> coll2, IEnumerable<T3> coll3, Func<T1, T2, T3, TResult> processor)
		{
			return coll1.Zip(coll2, Tuple.Create).Zip(coll3, (items1And2, item3) => processor(items1And2.Item1, items1And2.Item2, item3));
		} 

		private class TupleEqualityComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
		{
			public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
			{
				return x.Item2.Equals(y.Item2);
			}

			public int GetHashCode(Tuple<T1, T2> obj)
			{
				return obj.Item2.GetHashCode();
			}
		}
	}
}
