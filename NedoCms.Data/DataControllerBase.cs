using System;
using System.Web.Mvc;
using System.Web.Routing;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data
{
	/// <summary>
	/// Represents base data controller
	/// </summary>
	public abstract class DataControllerBase : Controller
	{
		/// <summary>
		/// Gets or sets data service instance
		/// </summary>
		public IDataService Data { get; private set; }

		/// <summary>
		/// Used to get data service specific for child of this controller
		/// </summary>
		/// <param name="requestContext">The HTTP context and route data.</param>
		/// <returns>Data service instance</returns>
		protected abstract IDataService GetDataService(RequestContext requestContext);

		/// <summary>
		/// Initializes data that might not be available when the constructor is called.
		/// </summary>
		/// <param name="requestContext">The HTTP context and route data.</param>
		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);

			if (Data == null)
			{
				Data = GetDataService(requestContext);
			}
		}

		/// <summary>
		/// Releases unmanaged resources and optionally releases managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (Data != null)
			{
				var disposable = Data as IDisposable;
				if (disposable != null) disposable.Dispose();

				Data = null;
			}

			base.Dispose(disposing);
		}
	}
}