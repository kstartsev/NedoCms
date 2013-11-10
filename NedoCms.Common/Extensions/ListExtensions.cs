using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	public static class ListExtensions
	{
		/// <summary>
		/// Gets all values based on type
		/// </summary>
		public static IEnumerable<SelectListItem> ConstList<T>(this HtmlHelper html, bool useRawAsValue = false)
		{
			return ConstListImpl(typeof(T), null, useRawAsValue);
		}

		/// <summary>
		/// Gets lsit of items from enum
		/// </summary>
		public static IEnumerable<SelectListItem> ConstList(this HtmlHelper html, object selected, bool useRawAsValue = false)
		{
			return ConstListImpl(selected.GetType(), selected, useRawAsValue);
		}

		/// <summary>
		/// Adds empty element on top of the given collection
		/// </summary>
		public static IEnumerable<SelectListItem> WithEmpty(this IEnumerable<SelectListItem> items, string empty = "Any")
		{
			yield return new SelectListItem { Text = empty, Value = string.Empty };
			foreach (var item in items)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Gets list of files from given folder matching specific pattern
		/// </summary>
		public static IEnumerable<SelectListItem> FilesFrom(this HtmlHelper helper, string root, string pattern = "*")
		{
			var mapped = helper.ViewContext.RequestContext.HttpContext.Server.MapPath(root);

			return Directory.EnumerateFiles(mapped, pattern)
				.Select(x => new { Name = Path.GetFileName(x), WithoutExt = Path.GetFileNameWithoutExtension(x) })
				.Select(x => new SelectListItem { Text = x.WithoutExt, Value = Path.Combine(root, x.Name) });
		}

		private static IEnumerable<SelectListItem> ConstListImpl(Type type, object selected, bool useRawAsValue = false)
		{
			if (!type.IsEnum) throw new InvalidEnumArgumentException();

			var underlyingType = Enum.GetUnderlyingType(type);
			var converted = selected != null ? Convert.ChangeType(selected, underlyingType) : null;

			var fields = type
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(f => f.IsLiteral && !f.IsInitOnly);

			return from field in fields
			       let display = field.GetCustomAttributes(typeof (DisplayAttribute), false).FirstOrDefault() as DisplayAttribute
			       let browsable = field.GetCustomAttributes(typeof (BrowsableAttribute), false).FirstOrDefault() as BrowsableAttribute
			       let visible = browsable.Get(x => x.Browsable, true)
			       let raw = field.GetRawConstantValue()
			       where visible
			       orderby display.Get(x => x.GetOrder(), int.MaxValue)
			       select new SelectListItem
			{
				Text = display.Get(x => x.GetName(), field.Name),
				Value = useRawAsValue ? raw.ToString() : field.Name,
				Selected = raw == converted
			};
		} 
	}
}