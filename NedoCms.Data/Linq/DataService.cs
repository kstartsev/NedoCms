using System.Data.Linq;

namespace NedoCms.Data.Linq
{
	/// <summary>
	/// Implements <see cref="DataServiceBase"/> with Linq to Sql data context as underlying data context.
	/// </summary>
	public class DataService<TDataContext> : DataServiceBase where TDataContext : DataContext, new()
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataService&lt;TDataContext&gt;"/> class.
		/// </summary>
		public DataService() : base(() => new TDataContext()) {}
	}
}