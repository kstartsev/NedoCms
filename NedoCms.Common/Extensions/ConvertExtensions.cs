using System;
using System.Globalization;

namespace NedoCms.Common.Extensions
{
	public static class ConvertExtensions
	{
		/// <summary>
		/// Converts model to specified type
		/// </summary>
		/// <typeparam name="TModel">Type of the model</typeparam>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="model">Model to convert</param>
		/// <param name="getter">Function to get result from model</param>
		/// <returns>Result taken from model</returns>
		public static T Get<TModel, T>(this TModel model, Func<TModel, T> getter) where TModel : class
		{
			return Get(model, getter, default(T));
		}

		/// <summary>
		/// Converts model to specified type
		/// </summary>
		/// <typeparam name="TModel">Type of the model</typeparam>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="model">Model to convert</param>
		/// <param name="getter">Function to get result from model</param>
		/// <param name="default">Default value</param>
		/// <returns>Result taken from model</returns>
		public static T Get<TModel, T>(this TModel model, Func<TModel, T> getter, T @default) where TModel : class
		{
			return model == null ? @default : getter(model);
		}

		/// <summary>
		/// Converts string to Guid
		/// </summary>
		public static Guid? Guid(this string s)
		{
			try
			{
				return new Guid(s);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Converts nullable decimal to string with specified format. Default format is 'N2'.
		/// If format is not specified than current culture formatting will be applied.
		/// </summary>
		public static string String(this decimal? d, string format = "N2")
		{
			return !d.HasValue
				       ? string.Empty
				       : (string.IsNullOrEmpty(format) ? d.Value.ToString(CultureInfo.CurrentCulture) : d.Value.ToString(format));
		}
	}
}