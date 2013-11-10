using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	public static class HtmlExtensions
	{
		/// <summary>
		/// Returns name for property of the model
		/// </summary>
		public static string NameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>(default(TModel)));
			return metadata.GetDisplayName();
		}
	}
}