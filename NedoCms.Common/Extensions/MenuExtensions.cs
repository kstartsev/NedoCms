using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Common.Models.Menu;

namespace NedoCms.Common.Extensions
{
	public static class MenuExtensions
	{
		/// <summary>
		/// Builds menu from provided menu description
		/// </summary>
		public static IEnumerable<Node> Menu(this HtmlHelper html, Descriptor[] items)
		{
			var url = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);

			return items.Select(x => ToMenuItem(x, url, html)).Where(x => x != null);
		}

		/// <summary>
		/// Converts descriptor to menu node
		/// </summary>
		private static Node ToMenuItem(Descriptor descriptor, UrlHelper url, HtmlHelper html)
		{
			if (descriptor == null) return null;
			if (descriptor.Hidden) return null;

			return new Node
			{
				Options = descriptor.Options,
				Url = url.RouteUrl(descriptor.Route, new { descriptor.Controller, descriptor.Action }),
				IsActive = IsActive(url, descriptor),
				Children = descriptor.Children != null
							   ? descriptor.Children.Safe().Select(x => ToMenuItem(x, url, html)).Where(x => x != null)
							   : Enumerable.Empty<Node>()
			};
		}

		/// <summary>
		/// Returns true if item with given descriptor is active
		/// </summary>
		private static bool IsActive(UrlHelper url, Descriptor descriptor)
		{
			var controller = url.RequestContext.RouteData.GetRequiredString("controller");
			var action = url.RequestContext.RouteData.GetRequiredString("action");

			var children = GetChildren(descriptor).ToList();

			return url.RequestContext.RouteData.Route == url.RouteCollection[descriptor.Route]
						&& children.Any(child => string.Equals(child.Item1, controller, StringComparison.OrdinalIgnoreCase)
												 && string.Equals(child.Item2, action, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Returns collection of children and subchildren for specific descriptor
		/// </summary>
		private static IEnumerable<Tuple<string, string>> GetChildren(Descriptor descriptor)
		{
			if (descriptor == null) yield break;

			yield return new Tuple<string, string>(descriptor.Controller, descriptor.Action);

			if (descriptor.Children != null)
			{
				//here we do not need to check for Hidden because even for hidden children their parent should be highlighted
				foreach (var action in descriptor.Children.SelectMany(GetChildren)) yield return action;
			}
		}
	}
}