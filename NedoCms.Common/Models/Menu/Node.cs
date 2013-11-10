using System.Collections.Generic;

namespace NedoCms.Common.Models.Menu
{
	/// <summary>
	/// Menu element
	/// </summary>
	public class Node
	{
		/// <summary>
		/// Options to render text in top menu
		/// </summary>
		public RenderOptions Options { get; set; }

		/// <summary>
		/// Redirection url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// true, if current item is active
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Sub menu items
		/// </summary>
		public IEnumerable<Node> Children { get; set; }
	}
}