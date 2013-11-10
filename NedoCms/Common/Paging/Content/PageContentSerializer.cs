using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using NedoCms.Common.Extensions;

namespace NedoCms.Common.Paging.Content
{
	public static class PageContentSerializer
	{
		public static string Serialize(MethodInfo method, PageContentAttribute attribute)
		{
			if (method == null || method.DeclaringType == null) throw new ArgumentNullException("method");
			if (attribute == null) throw new ArgumentNullException("attribute");

			var controller = method.DeclaringType.Name.Replace("Controller", "");

			// we want to keep information about edit and view actions
			var description = new PageContentDescription
			{
				Edit = new PageActionDescription {Controller = attribute.EditController, Action = attribute.EditAction},
				View = new PageActionDescription {Controller = controller, Action = method.Name},
				Description = method.EnumerateAttributes<DisplayAttribute>().Select(x => x.GetName()).FirstOrDefault().Either(method.Name)
			};

			return Serialize(description);
		}

		/// <summary>
		/// Returns base64 string, generated from provided information
		/// </summary>
		public static string Serialize<T>(T obj)
		{
			// saving to xml first, converting to base64 string after that
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				var serializer = new XmlSerializer(obj.GetType());
				serializer.Serialize(writer, obj);
			}
			var data = Encoding.UTF8.GetBytes(builder.ToString());
			return Convert.ToBase64String(data);
		}

		/// <summary>
		/// Returns object, deserialized from base64 string
		/// </summary>
		public static T Deserialize<T>(string data)
		{
			return Deserialize(data, () => default(T));
		}

		/// <summary>
		/// Returns object, deserialized from given base64 string. If failed to deserialize default object will be retrieved
		/// </summary>
		public static T Deserialize<T>(string data, Func<T> getdefault)
		{
			if (string.IsNullOrWhiteSpace(data)) return getdefault();

			var bytes = Convert.FromBase64String(data);
			var xml = Encoding.UTF8.GetString(bytes);

			using (var reader = new StringReader(xml))
			{
				var serializer = new XmlSerializer(typeof (T));
				var obj = serializer.Deserialize(reader);
				if (obj == null || !obj.GetType().IsAssignableFrom(typeof (T))) return getdefault();

				return (T) obj;
			}
		}
	}
}