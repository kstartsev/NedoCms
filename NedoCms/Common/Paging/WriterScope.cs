using System;
using System.IO;
using System.Web.Mvc;

namespace NedoCms.Common.Paging
{
	/// <summary>
	/// Simply replaces underlying writer for <see cref="HtmlHelper" /> with given writer
	/// </summary>
	internal sealed class WriterScope : IDisposable
	{
		private readonly HtmlHelper _helper;
		private readonly TextWriter _original;

		/// <summary>
		/// Creates new instance of scope, here writer is replaced
		/// </summary>
		/// <param name="helper">Helper where writer shoud be replaced</param>
		/// <param name="writer">Writer to use in place</param>
		public WriterScope(HtmlHelper helper, TextWriter writer)
		{
			_helper = helper;
			_original = _helper.ViewContext.Writer;

			_helper.ViewContext.Writer = writer;
		}

		/// <summary>
		/// Setting writer back to original
		/// </summary>
		public void Dispose()
		{
			_helper.ViewContext.Writer = _original;
		}
	}
}