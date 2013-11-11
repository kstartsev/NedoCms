using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using NedoCms.Data.Interfaces;
using NedoCms.Common.Extensions;

namespace NedoCms.Data.EntityFramework
{
	public class DataService<TDataContext> : IDataService, IDisposable where TDataContext : DbContext
	{
		private readonly TDataContext _context;

		public DataService(Func<TDataContext> factory)
		{
			if (factory == null) throw new ArgumentNullException("factory");

			_context = factory();
		}

		public IDataOptions Options
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public TextWriter Log
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
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

			using (var scope = new TransactionScope())
			{
				var result = operation(this);

				scope.Complete();

				return result;
			}
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}