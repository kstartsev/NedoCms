using System.Collections.Generic;

namespace NedoCms.Common.Models.Sorting
{
	/// <summary>
	/// Paged results
	/// </summary>
	public class PagedResult<T>
	{
		/// <summary>
		/// Gets/sets total number of items in query
		/// </summary>
		public int TotalItems { get; set; }

		/// <summary>
		/// Gets/sets total number of pages in query
		/// </summary>
		public int TotalPages { get; set; }

		/// <summary>
		/// Gets/sets currently selected page
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// Gets items retrived by query
		/// </summary>
		public IEnumerable<T> Items { get; set; }
	}
}