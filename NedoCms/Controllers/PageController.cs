using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using AutoMapper;
using NedoCms.Common.Attributes;
using NedoCms.Common.Extensions;
using NedoCms.Common.Models.Sorting;
using NedoCms.Common.Paging.Content;
using NedoCms.Controllers;
using NedoCms.Data;
using NedoCms.Data.Extensions;
using NedoCms.Data.Models;
using NedoCms.Models.Page;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PageController), "InitMapping")]

namespace NedoCms.Controllers
{
	/// <summary>
	/// Controller with page related actions
	/// </summary>
	public sealed partial class PageController : EfDataController<EditorDataContext>
	{
		public static void InitMapping()
		{
			Mapper.CreateMap<PageMetadata, PageMetadataModel>();
			Mapper.CreateMap<Page, PageModel>()
			      .ForMember(x => x.HasChildren, exp => exp.MapFrom(x => x.Children.Safe().Any()))
				  .ForMember(x => x.Metadata, exp => exp.MapFrom(x => x.PageMetadatas.Safe().Select(Mapper.Map<PageMetadata, PageMetadataModel>)));
		}

		/// <summary>
		/// Renders list of created pages
		/// </summary>
		[HttpGet]
		public ActionResult Index(SortableBase filter)
		{
			filter.OrderBy = filter.OrderBy.Either("MenuOrder");
			filter.OrderByDirection = filter.OrderByDirection.Either("asc");

			var pages = Data.Select<Page>().Where(x => x.ParentId == null).Sort(filter).ToList().Select(Mapper.Map<Page, PageModel>);

			return View(pages);
		}

		/// <summary>
		/// Renders children for requested page
		/// </summary>
		[HttpGet, Ajax]
		public ActionResult Children(Guid page, int level)
		{
			var items = Data.Select<Page>().Where(x => x.ParentId == page).OrderBy(x => x.MenuOrder).ToList().Select(Mapper.Map<Page, PageModel>);
			var model = Tuple.Create(items, level);

			return View("Index/Children", model);
		}

		/// <summary>
		/// Renders form for page edit
		/// </summary>
		[HttpGet, Ajax]
		public ActionResult Edit(Guid? id, Guid? parentid)
		{
			var model = Data.Select<Page>().Where(x => x.Id == id).ToList()
			                .Select(Mapper.Map<Page, PageModel>)
			                .FirstOr(new PageModel {ParentId = parentid});

			return View("Index/Edit", model);
		}

		/// <summary>
		/// Invoked on new page submit
		/// </summary>
		[HttpPost]
		public ActionResult Edit(PageModel model)
		{
			if (ModelState.IsValid && IsValidRoute(model.Id, model.Route))
			{
				Data.InTransaction(service =>
				{
					// updating or creating page
					var page = service.UpdateByIdOrInsert<Page>(model.Id, (x, insert) =>
					{
						x.Id = insert ? Guid.NewGuid() : x.Id;
						x.Title = model.Title;
						x.Type = model.Type;
						x.Master = model.Master;
						x.MenuLabel = model.MenuLabel;
						x.MenuOrder = model.MenuOrder ?? 1;
						x.Visible = model.Visible;
						x.Parent = model.ParentId.HasValue
									   ? service.Select<Page>().Single(_ => _.Id == model.ParentId)
							           : null;
						x.Route = ToUri(model.Route);
						x.FullRoute = RouteFor(x, model.Route); // this depends on parent, therefore it should be changed after parent modifications
					});

					// updating metadata
					service.Remove(page.PageMetadatas.Safe());
					service.Insert(model.Metadata.Safe().Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value)).Select(x => new PageMetadata
					{
						Id = Guid.NewGuid(),
						PageId = page.Id,
						Key = x.Key,
						Value = x.Value
					}));

					// updating child routes
					service.Update(UnwindChildren(page), x => x.FullRoute = RouteFor(x, x.Route));
				});
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Validates route for the page
		/// </summary>
		/// <param name="route">Suggested route</param>
		/// <param name="id">Id of the page</param>
		/// <returns>Validation result</returns>
		[HttpGet, Ajax]
		public JsonResult ValidateRoute(string route, Guid? id)
		{
			return !IsValidRoute(id, route)
				       ? Json("Page with requested route already specified.", JsonRequestBehavior.AllowGet)
				       : Json(true, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Removes page with specified id from data storage
		/// </summary>
		[HttpPost, Ajax]
		public ActionResult Delete(Guid id)
		{
			Data.InTransaction(service =>
			{
				service.Remove<PageContent>(x => x.PageId == id);
				service.Remove<PageMetadata>(x => x.PageId == id);
				service.Remove<Page>(x => x.Id == id);
			});

			return Json(true, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Renders page content edit view
		/// </summary>
		[HttpGet]
		public ActionResult Content(Guid id)
		{
			var page = Data.Select<Page>().FirstOrDefault(x => x.Id == id);
			if (page == null) return RedirectToAction("Index");

			return View(page);
		}

		/// <summary>
		/// Renders actual editor for page content, invoked as iframe source from <see cref="Content"/> action
		/// </summary>
		[HttpGet]
		public ActionResult ContentEditor(Guid id)
		{
			var page = Data.Select<Page>().First(x => x.Id == id);

			return View("Content/Edit", page.Master, page);
		}

		/// <summary>
		/// Renders toolbox with all actions marked with <see cref="PageContentAttribute"/>
		/// </summary>
		[ChildActionOnly, HttpGet]
		public ActionResult Toolbox()
		{
			var groups = BuildManager.GetReferencedAssemblies().Cast<Assembly>()
			                         .SelectMany(x => x.EnumerateTypes())
			                         .Where(x => x.IsController())
			                         .SelectMany(x => x.EnumerateActions())
			                         .Select(x => new
			                         {
				                         Method = x,
				                         Settings = x.EnumerateAttributes<PageContentAttribute>().FirstOrDefault(),
				                         Display = x.EnumerateAttributes<DisplayAttribute>().FirstOrDefault(),
			                         })
			                         .Where(x => x.Settings != null)
			                         .Select(x => new
			                         {
				                         Settings = PageContentSerializer.Serialize(x.Method, x.Settings),
				                         x.Settings.Group,
				                         Name = x.Display.Get(_ => _.GetName(), x.Method.Name)
			                         })
			                         .GroupBy(x => x.Group).OrderBy(x => x.Key)
			                         .Select(x => new ToolbarGroup(x.Key, x.Select(_ => new ToolbarItem
			                         {
				                         Settings = _.Settings,
				                         Name = _.Name
			                         }))).ToArray();

			return View("Content/Toolbox", groups);
		}

		/// <summary>
		/// Saves incoming content to data storage
		/// </summary>
		/// <param name="id">Id of the page</param>
		/// <param name="result">Collection of content items</param>
		/// <returns>Save result as json object</returns>
		[Ajax, HttpPost, ValidateInput(true)]
		public ActionResult SaveEditorContent(Guid id, PageContentCollection result)
		{
			Data.InTransaction(service =>
			{
				service.Remove<PageContent>(x => x.PageId == id);
				service.Insert(result.Get(x => x.Items).Safe().Select((x, i) => new PageContent
				{
					Id = x.Id ?? Guid.NewGuid(),
					PageId = id,
					PlaceHolder = x.PlaceHolder,
					Content = x.Content.Either(string.Empty), // here we assume that valid content is coming from client
					Order = i,
					Settings = x.Settings,
					SharedContentId = x.SharedId
				}));
			});
			return Json(true, JsonRequestBehavior.AllowGet);
		}

		private bool IsValidRoute(Guid? id, string route)
		{
			var page = id.HasValue
				           ? Data.Select<Page>(x => x.Id == id).Single()
				           : null;

			var r = RouteFor(page, route);

			return !Data.Select<Page>().Any(x => (id == null && x.FullRoute == r) || (x.Id != id && x.FullRoute == r));
		}

		private static string ToUri(string route)
		{
			return RouteHelper.ToUri(route.Safe().Trim('/'), true, true);
		}

		private static string RouteFor(Page page, string route)
		{
			var r = ToUri(route);

			if (page != null)
			{
				while ((page = page.Parent) != null)
				{
					r = page.Route + "/" + r;
				}
			}

			return r.ToLower();
		}

		private static IEnumerable<Page> UnwindChildren(Page page)
		{
			if (page == null) yield break;

			foreach (var child in page.Children.Safe())
			{
				yield return child;

				foreach (var inner in UnwindChildren(child))
				{
					yield return inner;
				}
			}
		}
	}
}