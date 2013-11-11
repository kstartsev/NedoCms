using System;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Data;
using NedoCms.Data.Models;
using NedoCms.Models;
using NedoCms.Models.Page;

namespace NedoCms.Controllers
{
	/// <summary>
	/// All public pages and static front end actions stored here
	/// </summary>
	public sealed class FrontController : EfDataController<EditorDataContext>
	{
		/// <summary>
		/// Renders page with specfied id
		/// </summary>
		public ActionResult Page(Guid id)
		{
			var page = Data.Select<Page>().FirstOrDefault(x => x.Id == id);
			if (page == null) return new EmptyResult();

			ViewBag.Title = page.Title;
			ViewBag.Metadata = page.PageMetadatas.Select(x => new PageMetadataModel
			{
				Key = x.Key,
				Value = x.Value
			}).ToArray();

			return View(null, page.Master, page);
		}

		/// <summary>
		/// Represents default content public viewer. Not implemented for now
		/// </summary>
		public ActionResult DefaultContentView(string text)
		{
			return new EmptyResult();
		}

		/// <summary>
		/// Represents default title renderer. Settings contains default title
		/// </summary>
		public ActionResult DefaultTitleView(string content)
		{
			return View(new TextModel {Text = string.Format("{0} - Page Editor", content)});
		}
	}
}