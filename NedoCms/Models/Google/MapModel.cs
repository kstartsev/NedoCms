using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NedoCms.Models.Google
{
	/// <summary>
	/// Describes google map
	/// </summary>
	public class MapModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MapModel"/> class.
		/// </summary>
		public MapModel()
		{
			Center = new MapPoint();
			Type = MapType.Roadmap;
		}

		/// <summary>
		/// Gets or sets center point on the map
		/// </summary>
		[Display(Name = "Center")]
		public MapPoint Center { get; set; }

		/// <summary>
		/// Gets or sets zoom level
		/// </summary>
		[Required, Display(Name = "Zoom"), Range(0, 21)]
		public int? Zoom { get; set; }

		/// <summary>
		/// Gets or sets selected map type
		/// </summary>
		[Required, Display(Name = "Map type")]
		public MapType Type { get; set; }

		/// <summary>
		/// Gets or sets collection of markers placed on the map
		/// </summary>
		[Display(Name = "Markers")]
		public IEnumerable<MapPoint> Markers { get; set; }
	}
}