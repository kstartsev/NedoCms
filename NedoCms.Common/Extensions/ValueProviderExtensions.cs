using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	/// <summary>
	/// Extensions for <see cref="IValueProvider"/> interface
	/// </summary>
	public static class ValueProviderExtensions
	{
		/// <summary>
		/// Gets value by key
		/// </summary>
		/// <typeparam name="T">Type of the value</typeparam>
		/// <param name="provider">Provider to get value</param>
		/// <param name="key">Key for value</param>
		/// <returns>Found value or default value for T</returns>
		public static T GetValue<T>(this IValueProvider provider, string key)
		{
			return GetValue(provider, key, default(T));
		}

		/// <summary>
		/// Gets value by key
		/// </summary>
		/// <typeparam name="T">Type of the value</typeparam>
		/// <param name="provider">Provider to get value</param>
		/// <param name="key">Key for value</param>
		/// <param name="default">Default value if no value was found for specified key</param>
		/// <returns>Found value or default value for T</returns>
		public static T GetValue<T>(this IValueProvider provider, string key, T @default)
		{
			try
			{
				var value = provider.GetValue(key);
				if (value == null) return @default;

				return (T) value.ConvertTo(typeof (T));
			}
			catch
			{
				return @default;
			}
		}
	}
}