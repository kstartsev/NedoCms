using System.ComponentModel.DataAnnotations;

namespace NedoCms.Models.Google
{
	/// <summary>
	/// Model describes single point on the map
	/// </summary>
	public class MapPoint
	{
		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		[Required, Display(Name = "Latitude")]
		public double? Latitude { get; set; }

		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		[Required, Display(Name = "Longitude")]
		public double? Longitude { get; set; }
	}
}