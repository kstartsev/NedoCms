using System.Data.Linq;
using System.Web.Routing;
using NedoCms.Data.Interfaces;
using NedoCms.Data.Linq;

namespace NedoCms.Data
{
	public class LinqDataController<TDataContext> : DataControllerBase where TDataContext : DataContext, new()
	{
		protected override IDataService GetDataService(RequestContext requestContext)
		{
			return new LinqDataService<TDataContext>(() => new TDataContext());
		}
	}
}