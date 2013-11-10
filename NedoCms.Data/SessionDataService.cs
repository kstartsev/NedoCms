using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using NedoCms.Common.Extensions;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data
{
	/// <summary>
	/// Naive data storage implementation on top of session object
	/// </summary>
	public sealed class SessionDataService : IDataService
	{
		private const string KeyPrefix = "F4A1B79190CB4012AAAD61E6DF88FC48";
		private readonly Controller _controller;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionDataService"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public SessionDataService(Controller controller)
		{
			_controller = controller;
		}

		public IDataOptions Options { get; set; }

		public TextWriter Log { get; set; }

		public void InTransaction(Action<IDataService> action)
		{
			action(this);
		}

		public TResult InTransaction<TResult>(Func<IDataService, TResult> operation)
		{
			return operation(this);
		}

		public IQueryable<TEntity> Select<TEntity>() where TEntity : class
		{
			return ItemsFor<TEntity>(_controller.Session).AsQueryable();
		}

		public TEntity SelectById<TEntity>(object id) where TEntity : class
		{
			throw new NotImplementedException();
		}

		public int Execute(string command, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var e = entities.Safe().ToList();

			ItemsFor<TEntity>(_controller.Session).AddRange(e);

			return e.Count;
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			return 0;
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> action) where TEntity : class
		{
			return 0;
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity, int> action) where TEntity : class
		{
			return 0;
		}

		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action) where TEntity : class
		{
			return 0;
		}

		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity, int> action) where TEntity : class
		{
			return 0;
		}

		public int Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var where = predicate.Compile();
			var items = ItemsFor<TEntity>(_controller.Session).Where(@where);

			return Remove(items);
		}

		public int Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			// original collection
			var items = ItemsFor<TEntity>(_controller.Session);
			var toremove = entities.Safe().ToList();

			// iterating via clone and removing from original collection
			var count = 0;
			foreach (var entity in items.ToList())
			{
				if (toremove.Contains(entity))
				{
					items.Remove(entity);
					count++;
				}
			}
			return count;
		}

		public void Dispose() {}

		/// <summary>
		/// Gets session key for given type
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <returns>Session key</returns>
		private static string KeyFor<T>()
		{
			return string.Format("{0}:{1}", KeyPrefix, typeof (T).FullName);
		}

		/// <summary>
		/// Gets items with specified type
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="session">Storage session</param>
		/// <returns>List of found entities. Using <see cref="List{T}"/> here to let user change collection without explicitly setting Session value</returns>
		private static List<T> ItemsFor<T>(HttpSessionStateBase session)
		{
			var key = KeyFor<T>();
			var items = session[key] as List<T>;
			if (items == null)
			{
				session[key] = items = new List<T>();
			}
			return items;
		}
	}
}