using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NedoCms.Models.Page
{
	/// <summary>
	/// Model describes page in the system
	/// </summary>
	public class PageModel
	{
		/// <summary>
		/// Gets or sets id of the page
		/// </summary>
		public Guid? Id { get; set; }

		/// <summary>
		/// Gets or sets id of the parent page
		/// </summary>
		[Display(Name = "Parent")]
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Gets or sets name of the page
		/// </summary>
		[Required]
		[RegularExpression(@"[^<>&\|\\\[\]\{\}\$~#%\*]*", ErrorMessage = "Invalid characters")]
		[Display(Name = "Title")]
		public string Title { get; set; }

		[Required]
		[Display(Name = "Page type")]
		public short Type { get; set; }

		/// <summary>
		/// Gets or sets the master file for the page
		/// </summary>
		[Required]
		[AllowHtml]
		[Display(Name = "Master file")]
		public string Master { get; set; }

		/// <summary>
		/// Indicates that page should be visible in front end
		/// </summary>
		[Required]
		[Display(Name = "Show in front end")]
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets menu text for the page
		/// </summary>
		[Required]
		[RegularExpression(@"[^<>&\|\\\[\]\{\}\$~#%\*]*", ErrorMessage = "Invalid characters")]
		[Display(Name = "Menu label")]
		public string MenuLabel { get; set; }

		/// <summary>
		/// Gets or sets page order in front end menu
		/// </summary>
		[Required]
		[Display(Name = "Menu order")]
		public int? MenuOrder { get; set; }

		/// <summary>
		/// Gets or sets front end route for page
		/// </summary>
		[Required]
		[RegularExpression(@"[^<>]*", ErrorMessage = "Invalid characters")]
		[Display(Name = "Page route")]
		[Remote("ValidateRoute", "Page", HttpMethod = "GET", AdditionalFields = "Id")]
		public string Route { get; set; }

		/// <summary>
		/// Indicates if page has children
		/// </summary>
		[Display(Name = "Has children")]
		public bool HasChildren { get; set; }

		/// <summary>
		/// Gets or sets metadata information for the page
		/// </summary>
		[Display(Name = "Metadata")]
		public IEnumerable<PageMetadataModel> Metadata { get; set; } 
	}
}