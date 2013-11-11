using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using NedoCms.Data.Interfaces;
using NedoCms.Data.Session;

namespace NedoCms.Data
{
	/// <summary>
	/// Represents data controller which stores information in session
	/// </summary>
	[SessionState(SessionStateBehavior.Required)]
	public class SessionDataController : DataControllerBase
	{
		/// <summary>
		/// Used to get data service specific for child of this controller
		/// </summary>
		/// <param name="requestContext">The HTTP context and route data.</param>
		/// <returns>Data service instance</returns>
		protected override IDataService GetDataService(RequestContext requestContext)
		{
			return new DataService(this);
		}
	}
}