using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	public static class ViewDataExtensions
	{
		/// <summary>
		/// Typed wrapper for adding error to model
		/// </summary>
		public static void AddModelError<TModel, TProperty>(this ViewDataDictionary data, Expression<Func<TModel, TProperty>> expression, Exception e)
		{
			AddModelError(data, expression, e.ToString());
		}

		/// <summary>
		/// Typed wrapper for adding error to model
		/// </summary>
		public static void AddModelError<TModel, TProperty>(this ViewDataDictionary data, Expression<Func<TModel, TProperty>> expression, string message)
		{
			var prop = expression.GetFullPropertyName();
			var key = data.TemplateInfo.GetFullHtmlFieldName(prop);

			data.ModelState.AddModelError(key, message);
		}
	}
}