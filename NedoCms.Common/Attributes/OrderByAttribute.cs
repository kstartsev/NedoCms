using System;

namespace NedoCms.Common.Attributes
{
	/// <summary>
	/// Lets user specify order by attribute
	/// </summary>
	public class OrderByAttribute : Attribute
	{
		public OrderByAttribute(string orderby)
		{
			OrderBy = orderby;
		}

		public string OrderBy { get; set; }
	}
}