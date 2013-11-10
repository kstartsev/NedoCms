using System;
using System.Web;

namespace NedoCms.Common.Extensions
{
	public static class RequestExtensions
	{
		/// <summary>
		/// Gets absolute URL out of current request and desired url
		/// </summary>
		public static string ToAbsolute(this HttpRequestBase request, string routeUrl)
		{
			if (request == null || request.Url == null) return routeUrl;

			return new Uri(new UriBuilder(request.IsSecureConnection ? "https" : "http", request.Url.Host, request.Url.Port, "/").Uri, routeUrl).AbsoluteUri;
		}

		/// <summary>
		/// Gets user IP
		/// </summary>
		public static string GetIp(this HttpRequestBase request)
		{
			if (request == null) return null;

			var ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(ip))
			{
				ip = request.ServerVariables["REMOTE_ADDR"];
			}
			return ip;
		} 
	}
}