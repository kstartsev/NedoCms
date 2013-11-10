using System.Configuration;

namespace NedoCms.Data.Models
{
	public partial class EditorDataContext
	{
		// TODO: let user specify connections string via config file
		public EditorDataContext() : this(ConfigurationManager.ConnectionStrings["DeusEx:ConnectionString"].ConnectionString) { }
	}
}