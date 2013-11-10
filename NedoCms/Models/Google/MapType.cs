using System.ComponentModel.DataAnnotations;

namespace NedoCms.Models.Google
{
	/// <summary>
	/// Contains possible map types
	/// </summary>
	public enum MapType
	{
		[Display(Name = "Roadmap")] Roadmap = 1,
		[Display(Name = "Satellite")] Satellite = 2,
		[Display(Name = "Terrain")] Terrain = 3,
		[Display(Name = "Hybrid")] Hybrid = 4
	}
}