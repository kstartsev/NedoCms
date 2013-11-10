using System;
using System.Linq;
using System.Linq.Dynamic;
using NedoCms.Common.Models.Sorting;

namespace NedoCms.Common.Extensions
{
	public static class SortingExtensions
	{
		/// <summary>
		/// Applies sorting to items based on sorting query, uses dynamic linq to generate sql query
		/// </summary>
		public static IQueryable<T> Sort<T>(this IQueryable<T> items, IOrderableQuery query)
		{
			try
			{
				var field = query.OrderBy;
				var direction = query.OrderByDirection ?? "asc";

				if (!string.IsNullOrEmpty(field))
				{
					return items.OrderBy(field + " " + direction);
				}

				return items;
			}
			catch
			{
				// ignoring sorting if something failed
				return items;
			}
		}

		/// <summary>
		/// Applies paging to list of items based on pagins query
		/// </summary>
		public static PagedResult<T> Paging<T>(this IQueryable<T> items, IPagingQuery query)
		{
			return Paging(items, query, x => x);
		}

		/// <summary>
		/// Applies paging and converts collection items
		/// </summary>
		public static PagedResult<TResult> Paging<TItem, TResult>(this IQueryable<TItem> items, IPagingQuery query, Func<TItem, TResult> convert)
		{
			var count = query.ItemsOnPage ?? 50;
			var index = query.CurrentPageIndex ?? 0;

			var total = items.Count();
			var totalPages = total / count;

			return new PagedResult<TResult>
			{
				CurrentPage = index,
				TotalItems = total,
				TotalPages = (total % count) != 0 ? totalPages + 1 : totalPages,
				Items = items.Skip(index * count).Take(count).Select(x => convert(x)).ToList()
			};
		}
	}
}