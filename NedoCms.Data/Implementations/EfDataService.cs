using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using NedoCms.Data.Interfaces;
using NedoCms.Common.Extensions;

namespace NedoCms.Data.Implementations
{
	public class EfDataService<TDataContext> : IDataService, IDisposable where TDataContext : DbContext
	{
		private readonly TDataContext _context;

		public EfDataService(Func<TDataContext> factory)
		{
			if (factory == null) throw new ArgumentNullException("factory");

			_context = factory();
		}

		public IQueryable<TEntity> Select<TEntity>() where TEntity : class
		{
			return _context.Set<TEntity>();
		}

		public TEntity SelectById<TEntity>(object id) where TEntity : class
		{
			return _context.Set<TEntity>().Find(id);
		}

		public int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			_context.Set<TEntity>().AddRange(entities);

			return _context.SaveChanges();
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			return Update(entities, _ => { });
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> action) where TEntity : class
		{
			return Update(entities, (entity, i) => action(entity));
		}

		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity, int> action) where TEntity : class
		{
			var set = _context.Set<TEntity>();
			var count = 0;
			foreach (var entity in entities)
			{
				action(set.Attach(entity), count);
				count++;
			}
			return _context.SaveChanges();
		}

		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action) where TEntity : class
		{
			return Update(predicate, (entity, i) => action(entity));
		}

		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity, int> action) where TEntity : class
		{
			var items = _context.Set<TEntity>().Where(predicate);
			var count = 0;
			foreach (var item in items)
			{
				action(item, count);
				count++;
			}
			return _context.SaveChanges();
		}

		public int Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var set = _context.Set<TEntity>();
			set.RemoveRange(set.Where(predicate));

			return _context.SaveChanges();
		}

		public int Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var set = _context.Set<TEntity>();

			set.RemoveRange(entities.Safe().Select(set.Attach));

			return _context.SaveChanges();
		}

		public void InTransaction(Action<IDataService> action)
		{
			InTransaction(x =>
			{
				action(x);
				return VoidResult.Instance;
			});
		}

		public TResult InTransaction<TResult>(Func<IDataService, TResult> operation)
		{
			if (operation == null) throw new ArgumentNullException("operation");

			// TransactioScope not supported in SQL Compact, so falling to default transaction implementation
			using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
			{
				try
				{
					var result = operation(this);

					transaction.Commit();

					return result;
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}