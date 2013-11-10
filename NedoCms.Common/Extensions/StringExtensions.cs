using System;
using System.Globalization;
using System.Text;

namespace NedoCms.Common.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Gets either given string or empty string if given one is null
		/// </summary>
		public static string Safe(this string s)
		{
			return s ?? string.Empty;
		}

		/// <summary>
		/// Gets either left or right if left is null or empty
		/// </summary>
		public static string Either(this string left, string right)
		{
			var l = left.Safe();
			var r = right.Safe();

			return string.IsNullOrEmpty(l) ? r : l;
		}

		/// <summary>
		/// Gets string with first letter in each word is capital
		/// </summary>
		public static string Capitalize(this string s)
		{
			var str = (s ?? string.Empty).Trim().ToLower();

			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str);
		}

		/// <summary>
		/// Gets string truncated to specified maxLength. Resulting string will be ended with given suffix if it truncated.
		/// </summary>
		public static string Truncate(this string s, int maxLength, string suffix = "...")
		{
			s = s.Safe();
			suffix = suffix.Safe();
			if (suffix.Length >= maxLength) throw new ArgumentException("Suffix length >= maxLength");

			if (s.Length > maxLength) return s.Substring(0, maxLength - suffix.Length) + suffix;

			return s;
		}

		/// <summary>
		/// Gets only text from string, all HTML tags will be omitted
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Plain(this string s)
		{
			if (string.IsNullOrEmpty(s)) return string.Empty;

			var inTag = false;
			var result = new StringBuilder();

			// loop through html, aggregating only viewable content
			foreach (var c in s)
			{
				if (c == '<')
				{
					inTag = true;
					continue;
				}

				if (c == '>')
				{
					inTag = false;
					continue;
				}

				if (!inTag)
				{
					result.Append(c);
				}
			}
			return result.ToString().Trim();
		}
	}
}