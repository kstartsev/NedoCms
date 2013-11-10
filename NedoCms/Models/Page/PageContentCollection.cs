namespace NedoCms.Models.Page
{
	/// <summary>
	/// Model decribes collection of page content items
	/// </summary>
	public class PageContentCollection
	{
		/// <summary>
		/// Gets or sets page content items
		/// </summary>
		public virtual PageContentModel[] Items { get; set; }
	}
}