using System.Collections.Generic;

namespace NedoCms.Models.Page
{
	/// <summary>
	/// Represents a toolbar item group.
	/// </summary>
	public class ToolbarGroup
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="items">The items.</param>
		public ToolbarGroup(string name, IEnumerable<ToolbarItem> items)
		{
			Name = name;
			Items = items;
		}

		/// <summary>
		/// Gets or sets group name
		/// </summary>
		public virtual string Name { get; private set; }

		/// <summary>
		/// Gets the items.
		/// </summary>
		public virtual IEnumerable<ToolbarItem> Items { get; private set; }
	}

	/// <summary>
	/// Represents a toolbar item.
	/// </summary>
	public class ToolbarItem
	{
		/// <summary>
		/// Gets or sets item name
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets data content
		/// </summary>
		public virtual string Settings { get; set; }
	}

}