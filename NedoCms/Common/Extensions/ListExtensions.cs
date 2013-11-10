// ReSharper disable ParameterTypeCanBeEnumerable.Local

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Data.Models;

namespace NedoCms.Common.Extensions
{
	/// <summary>
	/// 	Extensions to create dropdown lists
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Gets list of pages
		/// </summary>
		public static IEnumerable<SelectListItem> Pages(this HtmlHelper helper, Guid? selected)
		{
			var service = helper.GetDataService();

			var pages = service.Select<Page>().Select(x => new InternalPageModel { Id = x.Id, Parent = x.ParentId, Title = x.Title, Order = x.MenuOrder }).ToArray();
			var selectedid = selected.HasValue ? selected.Value.ToString() : "";

			return UnwindPages(pages, null, 0).ForEach(x => x.Selected = (x.Value == selectedid)).ToArray();
		}

		/// <summary>
		/// Gets list of master files from predefined folder
		/// </summary>
		/// <param name="helper">Html helper</param>
		/// <returns>List of found master files</returns>
		public static IEnumerable<SelectListItem> Masters(this HtmlHelper helper)
		{
			return helper.FilesFrom("~/Views/Master/", "*.cshtml").Select(x => new SelectListItem { Text = ConvertMasterName(x.Text), Value = x.Value });
		}

		/// <summary>
		/// Converts master files name to human readable form
		/// </summary>
		/// <param name="name">Name of master</param>
		/// <returns>Converted name</returns>
		/// <example>"Master$file" -> "Master - File"</example>
		private static string ConvertMasterName(string name)
		{
			return name.Replace("$", " - ").Capitalize();
		}

		/// <summary>
		/// Converts list of pages into list of items for dropdown
		/// </summary>
		/// <param name="pages">Pages from DB</param>
		/// <param name="parent">Current parent</param>
		/// <param name="level">Current inner level</param>
		/// <returns>List of converted items</returns>
		private static IEnumerable<SelectListItem> UnwindPages(InternalPageModel[] pages, Guid? parent, int level)
		{
			var items = pages.Where(x => (x.Parent == null && parent == null) || x.Parent == parent).OrderBy(x => x.Order);
			foreach (var item in items)
			{
				var prefix = new string('-', level);
				yield return new SelectListItem { Text = string.Format("{0} {1}", prefix, item.Title).Trim(), Value = item.Id.ToString() };

				foreach (var child in UnwindPages(pages, item.Id, level + 1))
				{
					yield return child;
				}
			}
		}

		/// <summary>
		/// Added this class to implement unwind for pages
		/// </summary>
		private sealed class InternalPageModel
		{
			public Guid Id { get; set; }
			public Guid? Parent { get; set; }
			public string Title { get; set; }
			public int Order { get; set; }
		}
	}
}

// ReSharper restore ParameterTypeCanBeEnumerable.Local