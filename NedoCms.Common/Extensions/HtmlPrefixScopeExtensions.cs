using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	/// <summary>
	/// Defines MVC prefix scope helper methods.
	/// </summary>
	public static class HtmlPrefixScopeExtensions
	{
		private const string ContextKey = "25C50B4FEBC14E3F9030C22DA7DA0B04-{0}";

		/// <summary>
		/// Begins the collection item.
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <param name="collectionName">Name of the collection.</param>
		/// <returns>The disposable collection item token.</returns>
		public static IDisposable BeginCollectionItem(this HtmlHelper html, string collectionName)
		{
			var context = GetContext(html.ViewContext.HttpContext, collectionName);

			context.Current++;

			var index = (context.Queue.Count > 0 ? context.Queue.Dequeue() : context.Current).ToString(CultureInfo.InvariantCulture);

			// autocomplete="off" is needed to work around a very annoying Chrome behavior whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
			html.ViewContext.Writer.WriteLine("<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />", collectionName, index);

			return new PrefixScope(html.ViewData.TemplateInfo, string.Format("{0}[{1}]", collectionName, index));
		}

		private static PrefixContext GetContext(HttpContextBase httpContext, string collectionName)
		{
			// We need to use the same sequence of IDs following a server-side validation failure,  
			// otherwise the framework won't render the validation error messages next to each item.
			var key = string.Format(ContextKey, collectionName);

			var context = (PrefixContext)httpContext.Items[key];
			if (context == null)
			{
				httpContext.Items[key] = context = new PrefixContext();

				var queue = httpContext.Request[collectionName + ".index"];

				if (!string.IsNullOrEmpty(queue))
				{
					foreach (var item in queue.Split(','))
					{
						context.Queue.Enqueue(int.Parse(item));
					}
				}
			}
			return context;
		}

		#region PrefixContext nested class

		private class PrefixContext
		{
			public int Current;									// ReSharper disable InconsistentNaming
			public readonly Queue<int> Queue;					// ReSharper restore InconsistentNaming

			public PrefixContext()
			{
				Current = -1;
				Queue = new Queue<int>();
			}
		}

		#endregion

		#region HtmlFieldPrefixScope nested class

		/// <summary>
		/// Represents a scope handle.
		/// </summary>
		private class PrefixScope : IDisposable
		{
			private readonly TemplateInfo _templateInfo;
			private readonly string _previousPrefix;

			/// <summary>
			/// Initializes a new instance of the <see cref="PrefixScope"/> class.
			/// </summary>
			/// <param name="templateInfo">The template info.</param>
			/// <param name="prefix">The HTML field prefix.</param>
			public PrefixScope(TemplateInfo templateInfo, string prefix)
			{
				if (templateInfo == null)
				{
					throw new ArgumentNullException("templateInfo");
				}
				if (string.IsNullOrEmpty(prefix))
				{
					throw new ArgumentNullException("prefix");
				}
				_templateInfo = templateInfo;

				_previousPrefix = templateInfo.HtmlFieldPrefix;

				templateInfo.HtmlFieldPrefix = prefix;
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_templateInfo.HtmlFieldPrefix = _previousPrefix;
			}
		}

		#endregion
	}
}