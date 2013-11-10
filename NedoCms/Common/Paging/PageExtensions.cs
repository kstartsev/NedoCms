using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using NedoCms.Common.Paging.Content;
using NedoCms.Models.Page;

namespace NedoCms.Common.Paging
{
	/// <summary>
	/// Defines helper methods for page management.
	/// </summary>
	public static class PageExtensions
	{
		private static readonly Guid Key = new Guid("{F1705466-C1AB-402E-B581-0CA81F63163C}");

		/// <summary>
		/// Renders the content of a named content section.
		/// </summary>
		public static HelperResult ContentSection(this WebViewPage page, string name)
		{
			return ContentSection(page, name, false);
		}

		/// <summary>
		/// Renders the content of a named content section.
		/// </summary>
		public static HelperResult ContentSection(this WebViewPage page, string name, bool required)
		{
			var renderer = page.PageData[Key] as Func<string, HelperResult>;

			return renderer != null ? renderer(name) : page.RenderSection(name, required);
		}

		/// <summary>
		/// Gets all the rendered content sections.
		/// </summary>
		public static void InitSectionRenderer(this WebViewPage page, Func<string, HelperResult> renderer)
		{
			page.PageData[Key] = renderer;
		}

		/// <summary>
		/// Renders editor for the specified page content item.
		/// </summary>
		public static MvcHtmlString EditorFor(this HtmlHelper html, PageContentModel content)
		{
			var settings = PageContentSerializer.Deserialize<PageContentDescription>(content.Settings);

			// if action or controller is not specified we will show default content editor
			if (string.IsNullOrWhiteSpace(settings.Edit.Action) || string.IsNullOrWhiteSpace(settings.Edit.Controller))
			{
				return html.Action("DefaultContentEditor", "Page", new {settings});
			}

			return html.Action(settings.Edit.Action, settings.Edit.Controller, new {content = content.Content, pageContentModel = content});
		}

		/// <summary>
		/// Returns description for given content model
		/// </summary>
		public static string DescriptionFor(this HtmlHelper html, PageContentModel content)
		{
			var settings = PageContentSerializer.Deserialize<PageContentDescription>(content.Settings);
			return settings.Description;
		}

		public static void RenderDisplayFor(this HtmlHelper html, PageContentModel content)
		{
			var description = PageContentSerializer.Deserialize<PageContentDescription>(content.Settings);

			// if action or controller is not specified we will show default content editor
			if (string.IsNullOrWhiteSpace(description.View.Action) || string.IsNullOrWhiteSpace(description.View.Controller))
			{
				html.RenderAction("DefaultContentView", "Front", new {text = description.Description});
				return;
			}

			var dictionary = new RouteValueDictionary();
			// we override 'content' field all the time, so we should avoid naming routes like that
			dictionary["content"] = content.Content;
			dictionary["pageContentModel"] = content;
			html.RenderAction(description.View.Action, description.View.Controller, dictionary);
		}
	}
}