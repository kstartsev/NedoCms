using System;
using System.Linq;
using System.Linq.Expressions;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data.Extensions
{
	public static class DataProviderExtensions
	{
		/// <summary>
		/// Shortcut for Select(..).Where(where)
		/// </summary>
		public static IQueryable<T> Select<T>(this IDataProvider service, Expression<Func<T, bool>> where) where T : class
		{
			return service.Select<T>().Where(where);
		}

		/// <summary>
		/// Updates DB item with matching id or inserts new item
		/// </summary>
		/// <typeparam name="T">Type of the item</typeparam>
		/// <param name="provider">Data provider</param>
		/// <param name="id">Id of the object</param>
		/// <param name="update">Action which should be invoked to update object settings.Second parameter indicates if object is inserted (true) or updated (false)</param>
		/// <returns>Number of updated/inserted entries</returns>
		public static T UpdateByIdOrInsert<T>(this IDataProvider provider, object id, Action<T, bool> update) where T : class, new()
		{
			var dbobj = id != null
				            ? provider.SelectById<T>(id)
				            : null;

			return UpdateOrInsert(provider, dbobj, update);
		}

		/// <summary>
		/// Updates DB item matching id or inserts new item
		/// </summary>
		/// <typeparam name="T">Type of the item</typeparam>
		/// <param name="provider">Data provider</param>
		/// <param name="dbobj">Object for update</param>
		/// <param name="update">Action which should be invoked to update object settings.Second parameter indicates if object is inserted (true) or updated (false)</param>
		/// <returns>Number of updated/inserted entries</returns>
		public static T UpdateOrInsert<T>(this IDataProvider provider, T dbobj, Action<T, bool> update) where T : class, new()
		{
			if (dbobj != null)
			{
				provider.Update(new[] { dbobj }, x => update(x, false));
				return dbobj;
			}

			var o = new T();
			update(o, true);
			provider.Insert(new[] { o });
			return o;
		}
	}
}