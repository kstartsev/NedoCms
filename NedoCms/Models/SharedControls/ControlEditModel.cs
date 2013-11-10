using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NedoCms.Models.SharedControls
{
	public class ControlEditModel
	{
		public virtual Guid? Id { get; set; }

		[Required]
		[Display(Name = "Name")]
		[StringLength(128)]
		[RegularExpression(@"[^<>&\|\\\[\]\{\}\$~#%\*]*", ErrorMessage = "Invalid characters")]
		public virtual string Name { get; set; }

		[Required]
		[Display(Name = "HTML")]
		[AllowHtml]
		public virtual string Content { get; set; }
	}
}