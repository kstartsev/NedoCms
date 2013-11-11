using System.Data.Entity;
using System.Web.Routing;
using NedoCms.Data.Implementations;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data
{
	public class EfDataController<TDataContext> : DataControllerBase where TDataContext : DbContext, new()
	{
		protected override IDataService GetDataService(RequestContext requestContext)
		{
			return new EfDataService<TDataContext>(() => new TDataContext());
		}
	}
}