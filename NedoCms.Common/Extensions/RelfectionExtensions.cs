using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace NedoCms.Common.Extensions
{
	public static class RelfectionExtensions
	{
		/// <summary>
		/// Returns collection of types defined in given assembly
		/// </summary>
		public static IEnumerable<Type> EnumerateTypes(this Assembly assembly)
		{
			if (assembly == null) return Enumerable.Empty<Type>();

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException exception)
			{
				return exception.Types;
			}
		}

		/// <summary>
		/// Returns collection of methods from given type. Those methods should public and return <see cref="ActionResult" /> therefore we can call them "action"
		/// </summary>
		public static IEnumerable<MethodInfo> EnumerateActions(this Type controller)
		{
			return controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
					   .Where(x => x.DeclaringType != typeof(object) && x.DeclaringType != typeof(ControllerBase) && x.DeclaringType != typeof(Controller))
					   .Where(x => !x.IsSpecialName && x.MemberType == MemberTypes.Method && typeof(ActionResult).IsAssignableFrom(x.ReturnType));
		}

		/// <summary>
		/// Returns collection of attributes with specified type for given member
		/// </summary>
		public static IEnumerable<TAttribute> EnumerateAttributes<TAttribute>(this MemberInfo member) where TAttribute : Attribute
		{
			return member == null
				       ? Enumerable.Empty<TAttribute>()
				       : member.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>();
		}

		/// <summary>
		/// Returns collection of attributes with specified type from given expression
		/// </summary>
		public static IEnumerable<TAttribute> EnumerateAttributes<TAttribute>(this LambdaExpression expression) where TAttribute : Attribute
		{
			return (expression.Body as MemberExpression).Get(x => x.Member).EnumerateAttributes<TAttribute>();
		}

		/// <summary>
		/// Returns true if given type is considered controller
		/// </summary>
		public static bool IsController(this Type type)
		{
			return type != null
				   && type.IsPublic
				   && !type.IsAbstract
				   && typeof(IController).IsAssignableFrom(type)
				   && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns full proeprty name
		/// </summary>
		// based on http://stackoverflow.com/questions/2789504/get-the-property-as-a-string-from-an-expressionfunctmodel-tproperty
		public static string GetFullPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> exp)
		{
			var member = FindMemberExpression(exp.Body);
			var expressions = member.Stream(x => FindMemberExpression(x.Expression))
			                        .TakeWhile(x => x != null)
			                        .Select(x => x.Member.Name).Reverse().ToArray();

			return string.Join(".", expressions.ToArray());
		}

		// code adjusted to prevent horizontal overflow
		private static MemberExpression FindMemberExpression(Expression exp)
		{
			if (exp == null) return null;

			var memberExp = exp as MemberExpression;
			if (memberExp != null) return memberExp;

			// if the compiler created an automatic conversion,
			// it'll look something like...
			// obj => Convert(obj.Property) [e.g., int -> object]
			// OR:
			// obj => ConvertChecked(obj.Property) [e.g., int -> long]
			// ...which are the cases checked in IsConversion
			if (IsConversion(exp) && exp is UnaryExpression)
			{
				memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
				if (memberExp != null)
				{
					return memberExp;
				}
			}

			return null;
		}

		private static bool IsConversion(Expression exp)
		{
			return exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked;
		}
	}
}