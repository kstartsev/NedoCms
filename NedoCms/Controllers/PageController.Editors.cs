using System;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Common.Attributes;
using NedoCms.Common.Extensions;
using NedoCms.Common.Paging.Content;
using NedoCms.Data.Models;
using NedoCms.Models;
using NedoCms.Models.Google;
using NedoCms.Models.Page;
using Newtonsoft.Json;

namespace NedoCms.Controllers
{
	public sealed partial class PageController
	{
		/// <summary>
		/// General view for any editor, basically it wraps specific editor in common interface
		/// </summary>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult EditorForPageContent(PageContentModel model)
		{
			// setting dummy content id right here, if it is still null
			model.Id = model.Id ?? Guid.NewGuid();

			return View("Editors/ContentEditor", model);
		}

		/// <summary>
		/// Default editor, renders view itself 
		/// </summary>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult DefaultContentEditor(PageContentDescription settings)
		{
			return View("Editors/Default", settings);
		}

		/// <summary>
		/// Editor for HTML content
		/// </summary>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult Html(string content, PageContentModel pageContentModel)
		{
			ViewBag.ContentModel = pageContentModel;

			return View("Editors/Html", new TextModel {Text = content.Either("Html content")});
		}

		/// <summary>
		/// Popup with HTML settings, right now it only shows shared controls
		/// </summary>
		[Ajax, HttpGet]
		public ActionResult HtmlPopup(Guid? sharedid)
		{
			ViewBag.SharedId = sharedid;

			return View("Popups/Html", Data.Select<SharedContent>()
			                               .OrderByDescending(x => x.ModificationDate).ToArray());
		}

		/// <summary>
		/// Saves information about html settings and returns information about selected shared content
		/// </summary>
		/// <returns>Content of the shared control</returns>
		[HttpPost]
		public ActionResult HtmlPopupSubmit(Guid? contentid)
		{
			var content = Data.Select<SharedContent>().FirstOrDefault(x => x.Id == contentid);

			return (content == null)
				       ? Json(new {Found = false, ContentId = contentid, Content = string.Empty}, JsonRequestBehavior.AllowGet)
				       : Json(new {Found = true, ContentId = contentid, Content = content.Content}, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Editor for script content
		/// </summary>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult Script(string content)
		{
			return View("Editors/Script", new TextModel {Text = content.Safe()});
		}

		/// <summary>
		/// Editor view for google map content
		/// </summary>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult GoogleMap(string content)
		{
			return View("Editors/GoogleMap", new TextModel {Text = content.Safe()});
		}

		/// <summary>
		/// Popup with google map editor settings
		/// </summary>
		/// <remarks>Using POST here because content value can be too long</remarks>
		[Ajax, HttpPost]
		public ActionResult GoogleMapPopup(string content)
		{
			return View("Popups/GoogleMap", JsonConvert.DeserializeObject<MapModel>(content));
		}

		/// <summary>
		/// Saves information about google map
		/// </summary>
		[HttpPost]
		public ActionResult GoogleMapSubmit(MapModel model)
		{
			return Json(JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);
		}
	}
}

// ReSharper restore RedundantAnonymousTypePropertyName