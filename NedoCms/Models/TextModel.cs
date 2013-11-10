using System.Web.Mvc;

namespace NedoCms.Models
{
	/// <summary>
	/// Simple text model, any text can be placed here
	/// </summary>
	public class TextModel
	{
		/// <summary>
		/// Text to be rendered on view
		/// </summary>
		[AllowHtml]
		public virtual string Text { get; set; } 
	}
}