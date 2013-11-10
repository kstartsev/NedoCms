using System;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	public static class TagBuilderExtensions
	{
		/// <summary>
		/// Sets id of the control and returns updated builder
		/// </summary>
		public static TagBuilder SetIdentity(this TagBuilder builder, string name)
		{
			return UpdateAndReturn(builder, x => x.GenerateId(name));
		}

		/// <summary>
		/// Adds attribute to the builder and returns updated builder
		/// </summary>
		public static TagBuilder WithAttribute(this TagBuilder builder, string key, string value)
		{
			return builder.WithAttribute(key, value, false);
		}

		/// <summary>
		/// Adds attribute to the builder and returns updated builder
		/// </summary>
		public static TagBuilder WithAttribute(this TagBuilder builder, string key, string value, bool replaceExisting)
		{
			return UpdateAndReturn(builder, x => x.MergeAttribute(key, value, replaceExisting));
		}

		/// <summary>
		/// Set inner html for the builder and returns this builder
		/// </summary>
		public static TagBuilder SetInnerHtml(this TagBuilder builder, string html)
		{
			return UpdateAndReturn(builder, x => x.InnerHtml = html);
		}

		/// <summary>
		/// Converts given builder to <see cref="MvcHtmlString" />
		/// </summary>
		public static MvcHtmlString ToMvcHtmlString(this TagBuilder builder)
		{
			return new MvcHtmlString(builder.ToString());
		}

		/// <summary>
		/// Applies action to given builder and returns builder with updated state
		/// </summary>
		private static TagBuilder UpdateAndReturn(TagBuilder builder, Action<TagBuilder> action)
		{
			action(builder);
			return builder;
		}
	}
}