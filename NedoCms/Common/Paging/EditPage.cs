using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using System.Web.WebPages;
using NedoCms.Common.Extensions;
using NedoCms.Models.Page;
using Page = NedoCms.Data.Models.Page;

namespace NedoCms.Common.Paging
{
	/// <summary>
	/// Represents a edit mode for a page.
	/// </summary>
	public abstract class EditPage<TPage> : WebViewPage<TPage> where TPage : Page
	{
		/// <summary>
		/// Runs the page hierarchy for the ASP.NET Razor execution pipeline.
		/// </summary>
		public override void ExecutePageHierarchy()
		{
			var groups = Model.Get(x => x.PageContents).Safe().GroupBy(x => x.PlaceHolder);
			this.InitSectionRenderer(name => new HelperResult(x =>
			{
				using (var writer = new HtmlTextWriter(x))
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "page-content-item");
					writer.AddAttribute("data-placeholder-name", name);

					// starting container
					writer.RenderBeginTag(HtmlTextWriterTag.Div);

					// adding editors for already added controls
					var g = groups.FirstOrDefault(gr => gr.Key == name);
					if (g != null)
					{
						var editors = g.OrderBy(_ => _.Order);
						using (new WriterScope(Html, writer))
						{
							foreach (var editor in editors)
							{
								Html.RenderAction("EditorForPageContent", "Page", new
								{
									model = new PageContentModel
									{
										Id = editor.Id,
										PageId = editor.PageId,
										Content = editor.Content,
										PlaceHolder = editor.PlaceHolder,
										Settings = editor.Settings,
										SharedId = editor.SharedContentId
									}
								});
							}
						}
					}

					// adding drop area
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "page-content-item-drop-area");
					writer.AddAttribute(HtmlTextWriterAttribute.Title, name);
					writer.AddAttribute("ondrop", "onDrop(event); return true;");
					writer.AddAttribute("ondragover", "onDragOver(event); return true;");
					writer.AddAttribute("ondragleave", "onDragLeave(event); return true;");
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
					writer.Write("Drop actions here..");
					writer.RenderEndTag();

					// closing container
					writer.RenderEndTag();
				}
			}));

			base.ExecutePageHierarchy();
		}
	}
}