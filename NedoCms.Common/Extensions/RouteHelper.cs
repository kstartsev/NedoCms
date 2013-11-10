using System;
using System.Collections.Generic;
using System.Linq;

namespace NedoCms.Common.Extensions
{
	public class RouteHelper
	{
		/// <summary>
		/// Converts given string to correct URI
		/// </summary>
		/// <param name="uri">Suggested URI</param>
		/// <param name="letbrackets">If true, { and } brackets will be left as is</param>
		/// <returns>Correct URI</returns>
		public static string ToUri(string uri, bool letbrackets = false, bool letslash = false)
		{
			if (uri == null) throw new ArgumentNullException("uri");

			var builder = new System.Text.StringBuilder();

			foreach (var c in uri)
			{
				switch (c)
				{
					case '/':
						if (letslash) builder.Append("/");
						else builder.Append("-");
						break;
					case 'Å':
					case 'å':
					case 'Ä':
					case 'ä':
						builder.Append('a');
						break;
					case 'Ö':
					case 'ö':
						builder.Append('o');
						break;
					// ampersand leads to bad request error in IIS for security reasons.
					case '&':
						builder.Append("and");
						break;
					case '.':
					case ',':
					case ';':
					case '-':
					case '=':
					case '!':
						//do not need these symbols
						builder.Append("-");
						break;
					default:
						if (char.IsLetterOrDigit(c)) builder.Append(char.ToLower(c));
						else if (!char.IsControl(c))
						{
							// { and } are here to handle page with "Route" type
							if (letbrackets && (c == '{' || c == '}')) builder.Append(c);
							else builder.Append('-');
						}
						break;
				}
			}

			return builder.ToString();
		}

		public static string GetUniqueRoute(string rawRoute, IEnumerable<string> existingURIs)
		{
			var route = ToUri(rawRoute);

			var index = 0;
			var suffix = string.Empty;
			while (existingURIs.Any(x => x == route + suffix))
			{
				index++;
				suffix = "-" + index;
			}

			return (route + suffix).ToLower();
		}
	}
}