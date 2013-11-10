using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using NedoCms.Common.Extensions;
using NedoCms.Common.Paging.Content;
using NedoCms.Data.Models;
using NedoCms.Models.Page;

namespace NedoCms.Common.Paging
{
	/// <summary>
	/// Represents a display for a page.
	/// </summary>
	public abstract class ViewPage<TPage> : WebViewPage<TPage> where TPage : Page
	{
		/// <summary>
		/// Runs the page hierarchy for the ASP.NET Razor execution pipeline.
		/// </summary>
		public override void ExecutePageHierarchy()
		{
			var page = ViewData.Model;
			if (page != null)
			{
				var groups = page.PageContents.GroupBy(x => x.PlaceHolder);
				this.InitSectionRenderer(name => new HelperResult(writer =>
				{
					var g = groups.FirstOrDefault(x => x.Key == name);
					if (g == null)
					{
						// if there is not special title in items, add standard title
						if (name == "TitleContent")
						{
							RenderContent(writer, new PageContent
							{
								Id = Guid.NewGuid(),
								PageId = page.Id,
								Settings = PageContentSerializer.Serialize(new PageContentDescription
								{
									// we do not need other fields
									View = new PageActionDescription {Action = "DefaultTitleView", Controller = "Front"}
								}),
								Content = page.Title
							}.Enumerate());
						}
						return;
					}
					RenderContent(writer, g.OrderBy(x => x.Order));
				}));

				base.ExecutePageHierarchy();
			}
		}

		private void RenderContent(TextWriter writer, IEnumerable<PageContent> contents)
		{
			using (new WriterScope(Html, writer))
			{
				foreach (var item in contents)
				{
					Html.RenderDisplayFor(new PageContentModel
					{
						Id = item.Id,
						PageId = item.PageId,
						Content = item.Content,
						PlaceHolder = item.PlaceHolder,
						Settings = item.Settings,
						SharedId = item.SharedContentId
					});
				}
			}
		}
	}
}