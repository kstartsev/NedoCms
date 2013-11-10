using System;
using System.ComponentModel.DataAnnotations;

namespace NedoCms.Models.SharedControls
{
	public class ControlListModel
	{
		public virtual Guid Id { get; set; }

		[Display(Name = "Name")]
		public virtual string Name { get; set; }

		[Display(Name = "Last change")]
		public virtual DateTime ModificationDate { get; set; }
	}
}