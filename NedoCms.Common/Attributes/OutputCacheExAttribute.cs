using System.Configuration;
using System.Web.Mvc;
using System.Web.WebPages;

namespace NedoCms.Common.Attributes
{
	/// <summary>
	/// Basic extended cache attribute
	/// </summary>
	public class OutputCacheExAttribute : OutputCacheAttribute
	{
		private bool _active;

		public OutputCacheExAttribute()
		{
			Active = true;
			Duration = 300;
			VaryByParam = "*";
		}

		public bool Active
		{
			get
			{
				// adding this in order to be able to disable caching quickly
				var disableOutputCache = ConfigurationManager.AppSettings["DisableOutputCache"];
				if (string.IsNullOrWhiteSpace(disableOutputCache) || !disableOutputCache.AsBool())
				{
					return _active;
				}

				return false;
			}
			set { _active = value; }
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsLocal) return;
			if (!Active) return;

			base.OnActionExecuted(filterContext);
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsLocal) return;
			if (!Active) return;

			base.OnActionExecuting(filterContext);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsLocal) return;
			if (!Active) return;

			base.OnResultExecuted(filterContext);
		}

		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsLocal) return;
			if (!Active) return;

			base.OnResultExecuting(filterContext);
		}
	}
}