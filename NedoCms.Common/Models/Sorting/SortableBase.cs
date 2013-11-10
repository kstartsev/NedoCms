namespace NedoCms.Common.Models.Sorting
{
	/// <summary>
	/// Base implementation for sortable/paginable queries 
	/// </summary>
	public class SortableBase : IOrderableQuery, IPagingQuery
	{
		#region IOrderableQuery Members

		public virtual string OrderBy { get; set; }
		public virtual string OrderByDirection { get; set; }

		#endregion

		#region IPagingQuery Members

		public virtual int? CurrentPageIndex { get; set; }
		public virtual int? ItemsOnPage { get; set; }

		#endregion
	}
}