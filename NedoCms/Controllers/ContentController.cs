using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NedoCms.Common.Paging.Content;
using NedoCms.Data;
using NedoCms.Models;
using NedoCms.Models.Google;
using Newtonsoft.Json;

namespace NedoCms.Controllers
{
	/// <summary>
	/// 	Generic content container with generic actions
	/// </summary>
	public sealed class ContentController : SessionDataController
	{
		/// <summary>
		/// 	This is basic HTML content item
		/// </summary>
		[PageContent(Group = "Generic content", EditAction = "Html", EditController = "Page"), Display(Name = "Html")]
		public ActionResult Html(string content)
		{
			return View(new TextModel {Text = content});
		}

		/// <summary>
		/// 	Adds scripts to the page. Using this because CkEditor encodes scripts
		/// </summary>
		[PageContent(Group = "Generic content", EditAction = "Script", EditController = "Page"), Display(Name = "Script")]
		public ActionResult Script(string content)
		{
			return View(new TextModel {Text = content});
		}

		/// <summary>
		/// 	Renders google map based on settigs
		/// </summary>
		[PageContent(Group = "Generic content", EditAction = "GoogleMap", EditController = "Page"), Display(Name = "Google map")]
		public ActionResult GoogleMap(string content)
		{
			return View(JsonConvert.DeserializeObject<MapModel>(content));
		}
	}
}