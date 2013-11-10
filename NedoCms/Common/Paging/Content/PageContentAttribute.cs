using System;

namespace NedoCms.Common.Paging.Content
{
	/// <summary>
	/// Indicates an action based page content.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class PageContentAttribute : Attribute
	{
		/// <summary>
		/// Gets the edit action.
		/// </summary>
		public string EditAction { get; set; }

		/// <summary>
		/// Gets the edit controller.
		/// </summary>
		public string EditController { get; set; }

		/// <summary>
		/// Specified toolbar group
		/// </summary>
		public string Group { get; set; }
	}
}