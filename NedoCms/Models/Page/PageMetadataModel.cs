using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NedoCms.Models.Page
{
	/// <summary>
	/// Model describing page metadata
	/// </summary>
	public class PageMetadataModel
	{
		/// <summary>
		/// Gets or sets key
		/// </summary>
		[Display(Name = "Key")]
		[RegularExpression(@"[^<>&\|\\\[\]\{\}\$~#%\*]*", ErrorMessage = "Invalid characters")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets values
		/// </summary>
		[Display(Name = "Value")]
		[AllowHtml]
		public string Value { get; set; }
	}
}