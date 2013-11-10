using System;

namespace NedoCms.Models.Page
{
	/// <summary>
	/// Model describes content item on the page
	/// </summary>
	public class PageContentModel
	{
		/// <summary>
		/// Id of the page where this content was placed
		/// </summary>
		public virtual Guid PageId { get; set; }

		/// <summary>
		/// Id of the content itself
		/// </summary>
		public virtual Guid? Id { get; set; }

		/// <summary>
		/// Placeholder where content is located
		/// </summary>
		public virtual string PlaceHolder { get; set; }

		/// <summary>
		/// String contaning action specific content
		/// </summary>
		public virtual string Content { get; set; }

		/// <summary>
		/// Base64 string containing action settings like action description etc
		/// </summary>
		public virtual string Settings { get; set; }

		/// <summary>
		/// Gets or sets shared content id
		/// </summary>
		public virtual Guid? SharedId { get; set; }
	}
}