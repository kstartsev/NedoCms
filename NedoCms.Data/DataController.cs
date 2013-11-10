using System.Data.Linq;
using System.Web.Routing;
using NedoCms.Data.Interfaces;
using NedoCms.Data.Linq;

namespace NedoCms.Data
{
	/// <summary>
	/// Defines a base controller for Deus actions.
	/// </summary>
	public class DataController<TDataContext> : DataControllerBase where TDataContext : DataContext, new()
	{
		/// <summary>
		/// Used to get data service specific for child of this controller
		/// </summary>
		/// <param name="requestContext">The HTTP context and route data.</param>
		/// <returns>Data service instance</returns>
		protected override IDataService GetDataService(RequestContext requestContext)
		{
			return new DataService<TDataContext>();
		}
	}
}