using System.Collections.Generic;

namespace NedoCms.Models.CkEditor
{
	/// <summary>
	/// Model describes uploaded images
	/// </summary>
	public sealed class ImagesModel : EditorModel
	{
		/// <summary>
		/// Gets or sets list of uploaded images.
		/// </summary>
		public IEnumerable<ImageItemModel> Images { get; set; }
	}

	/// <summary>
	/// Model for single image
	/// </summary>
	public sealed class ImageItemModel
	{
		/// <summary>
		/// Gets or sets image Url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets image name.
		/// </summary>
		public string Name { get; set; }
	}
}