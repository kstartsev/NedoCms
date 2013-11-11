using System.Data.Entity;
using System.Web.Routing;
using NedoCms.Data.EntityFramework;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data
{
	public class EfDataController<TDataContext> : DataControllerBase where TDataContext : DbContext, new()
	{
		protected override IDataService GetDataService(RequestContext requestContext)
		{
			return new DataService<TDataContext>(() => new TDataContext());
		}
	}
}