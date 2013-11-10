using System.IO;

namespace NedoCms.Common.Images
{
	/// <summary>
	/// Represents the result of image resolving operation.
	/// </summary>
	public class ResolveImageResult
	{
		/// <summary>
		/// Gets or sets the stream.
		/// </summary>
		public Stream Stream { get; set; }

		/// <summary>
		/// Gets or sets the type of the MIME.
		/// </summary>
		public string MimeType { get; set; }
	}
}