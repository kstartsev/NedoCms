using System;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Common.Attributes;
using NedoCms.Common.Extensions;
using NedoCms.Common.Models.Sorting;
using NedoCms.Data.Extensions;
using NedoCms.Data.Models;
using NedoCms.Models.SharedControls;

namespace NedoCms.Controllers
{
	public sealed partial class PageController
	{
		/// <summary>
		/// Renders list of shared controls
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Shared(SortableBase filter)
		{
			filter.OrderBy = filter.OrderBy.Either("ModificationDate");
			filter.OrderByDirection = filter.OrderByDirection.Either("desc");
			filter.ItemsOnPage = filter.ItemsOnPage ?? 50;

			var items = Data.Select<SharedContent>().Select(x => new ControlListModel
			{
				Id = x.Id,
				Name = x.Name,
				ModificationDate = x.ModificationDate
			}).Sort(filter).ToArray(); // most likely no one will have many shared controls, therefore we do not need pagination

			return View(items);
		}

		/// <summary>
		/// Shows shared control details
		/// </summary>
		[Ajax, HttpGet]
		public ActionResult SharedDetails(Guid? id)
		{
			var shared = Data.Select<SharedContent>().FirstOrDefault(x => x.Id == id);
			var model = shared != null
				            ? new ControlEditModel {Id = shared.Id, Name = shared.Name, Content = shared.Content}
				            : new ControlEditModel {Id = id};

			return View("Controls/Details", model);
		}

		/// <summary>
		/// Receives shared control details after modifications
		/// </summary>
		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult SharedDetails(ControlEditModel model)
		{
			if (ModelState.IsValid)
			{
				Data.InTransaction(service =>
				{
					var sc = service.UpdateByIdOrInsert<SharedContent>(model.Id, (content, insert) =>
					{
						if (insert)
						{
							content.Id = Guid.NewGuid();
							content.CreationDate = DateTime.UtcNow;
						}
						content.ModificationDate = DateTime.UtcNow;
						content.Name = model.Name;
						content.Content = model.Content;
					});
					service.Update<PageContent>(x => x.SharedContentId == sc.Id, x => x.Content = sc.Content);
				});
			}
			return RedirectToAction("Shared");
		}

		/// <summary>
		/// Removes shared control from the system and removes connection between this control and simple content controls
		/// </summary>
		[Ajax, HttpPost]
		public ActionResult DeleteShared(Guid id)
		{
			Data.InTransaction(service =>
			{
				service.Update<PageContent>(x => x.SharedContentId == id, x => x.SharedContent = null);
				service.Remove<SharedContent>(x => x.Id == id);
			});

			return Json(true, JsonRequestBehavior.AllowGet);
		}
	}
}