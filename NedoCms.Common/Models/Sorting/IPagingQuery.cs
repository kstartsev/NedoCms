namespace NedoCms.Common.Models.Sorting
{
	/// <summary>
	/// Information for paging
	/// </summary>
	public interface IPagingQuery
	{
		/// <summary>
		/// Currently selected list
		/// </summary>
		int? CurrentPageIndex { get; set; }

		/// <summary>
		/// Items per page
		/// </summary>
		int? ItemsOnPage { get; set; }
	}
}