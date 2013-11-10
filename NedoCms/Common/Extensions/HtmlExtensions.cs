using System;
using System.Web.Mvc;
using NedoCms.Data;
using NedoCms.Data.Interfaces;

namespace NedoCms.Common.Extensions
{
	public static class HtmlExtensions
	{
		/// <summary>
		/// Gets controller from context
		/// </summary>
		public static DataControllerBase GetController(this HtmlHelper helper)
		{
			var dataController = helper.ViewContext.Controller as DataControllerBase;
			if (dataController == null) throw new InvalidOperationException("Can't get DataController");

			return dataController;
		}

		/// <summary>
		/// Gets data service from context
		/// </summary>
		public static IDataProvider GetDataService(this HtmlHelper helper)
		{
			return GetController(helper).Data;
		} 
	}
}