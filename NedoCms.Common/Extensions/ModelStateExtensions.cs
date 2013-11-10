using System.Collections.Generic;
using System.Web.Mvc;
using NedoCms.Common.Models;

namespace NedoCms.Common.Extensions
{
	public static class ModelStateExtensions
	{
		/// <summary>
		/// Gets list of errors from model state
		/// </summary>
		public static IEnumerable<ErrorModel> UnwindErrors(this ModelStateDictionary dictionary)
		{
			if (dictionary == null || dictionary.IsValid) yield break;

			foreach (var key in dictionary.Keys)
			{
				ModelState value;
				if (!dictionary.TryGetValue(key, out value)) continue;

				if (value.Errors.Count == 0) continue;

				foreach (var error in value.Errors)
				{
					yield return new ErrorModel { Key = key, Message = error.ErrorMessage };
				}
			}
		}
	}
}