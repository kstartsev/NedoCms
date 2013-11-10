using System.Web.Routing;

namespace NedoCms.Common.Extensions
{
	public static class RouteValueDictionaryExtensions
	{
		/// <summary>
		/// Converts object to route dictionary
		/// </summary>
		public static RouteValueDictionary AsRouteDictionary(this object o)
		{
			return o == null ? new RouteValueDictionary() : new RouteValueDictionary(o);
		}

		/// <summary>
		/// Set value for specific key and returns updated dictionary
		/// </summary>
		public static RouteValueDictionary Set(this RouteValueDictionary dictionary, string key, object value)
		{
			dictionary[key] = value;
			return dictionary;
		}
	}
}