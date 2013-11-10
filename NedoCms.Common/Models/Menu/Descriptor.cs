namespace NedoCms.Common.Models.Menu
{
	/// <summary>
	/// Based on information in this description instance of the <see cref="Node"/> will be created 
	/// </summary>
	public class Descriptor
	{
		/// <summary>
		/// Contains rendering options for menu item
		/// </summary>
		public RenderOptions Options { get; set; }

		/// <summary>
		/// Route name used to create url
		/// </summary>
		public string Route { get; set; }

		/// <summary>
		/// Controller which is used to create <see cref="Node.Url"/>
		/// </summary>
		public string Controller { get; set; }

		/// <summary>
		/// Controller's action
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// Do not show in menu if hidden
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Child items of the menu element
		/// </summary>
		public Descriptor[] Children { get; set; }
	}
}