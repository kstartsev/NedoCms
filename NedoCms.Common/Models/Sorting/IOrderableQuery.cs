namespace NedoCms.Common.Models.Sorting
{
	/// <summary>
	/// Contains information for ordered lists
	/// </summary>
	public interface IOrderableQuery
	{
		/// <summary>
		/// Gets/sets field which is used in order expression
		/// </summary>
		string OrderBy { get; set; }

		/// <summary>
		/// Gets/sets order by direction
		/// </summary>
		string OrderByDirection { get; set; }
	}
}