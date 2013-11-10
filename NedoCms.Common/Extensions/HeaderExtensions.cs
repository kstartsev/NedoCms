using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using NedoCms.Common.Attributes;

namespace NedoCms.Common.Extensions
{
	public static class HeaderExtensions
	{
		/// <summary>
		/// Renders header for specified expression
		/// </summary>
		public static MvcHtmlString HeaderFor<TItem, TValue>(this IEnumerable<TItem> html, Expression<Func<TItem, TValue>> expression, string text = null)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TItem>(default(TItem)));
			var name = metadata.GetDisplayName();

			return new MvcHtmlString(text.Either(name));
		}

		/// <summary>
		/// Returns a link for the specified collection string header for the specified expression.
		/// </summary>
		public static MvcHtmlString OrderableHeaderFor<TItem, TValue>(this IEnumerable<TItem> html, Expression<Func<TItem, TValue>> expression, string text = null)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TItem>(default(TItem)));
			var explicitOrderBy = expression.EnumerateAttributes<OrderByAttribute>().Safe().FirstOrDefault().Get(x => x.OrderBy);
			var name = ("orderby." + explicitOrderBy.Either(ExpressionHelper.GetExpressionText(expression))).ToLower();

			return new TagBuilder("a")
				.SetIdentity(name.ToLower())
				.WithAttribute("href", "javascript:void(0)")
				.WithAttribute("name", name.ToLower())
				.SetInnerHtml(text.Either(metadata.GetDisplayName()))
				.ToMvcHtmlString();
		}
	}
}