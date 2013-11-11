using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using NedoCms.Data.Extensions;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data.Linq
{
	/// <summary>
	/// Implements <see cref="IDataService"/> with Linq to Sql data context as underlying data context.
	/// </summary>
	public class DataService<TDataContext> : IDataService, IDisposable where TDataContext : DataContext, new()
	{
		private readonly TDataContext _context;

		private DataOptions _dataOptions;
		private System.IO.TextWriter _log;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataService{TDataContext}"/> class.
		/// </summary>
		/// <param name="createContext">The create context.</param>
		public DataService(Func<TDataContext> createContext)
		{
			if (createContext == null) throw new ArgumentNullException("createContext");

			_context = createContext();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_context.Dispose();

			if (_log != null)
			{
				_log.Dispose();
			}
		}

		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public IDataOptions Options
		{
			get { return _dataOptions; }
			set
			{
				_dataOptions = value as DataOptions;

				if (_context == null)
				{
					return;
				}
				_context.LoadOptions = _dataOptions != null ? _dataOptions.LoadOptions : null;
			}
		}

		/// <summary>
		/// Gets or sets the log.
		/// </summary>
		/// <value>The log.</value>
		public System.IO.TextWriter Log
		{
			get
			{
				return _log;
			}
			set
			{
				_log = value;

				if (_context == null)
				{
					return;
				}

				_context.Log = value;
			}
		}

		/// <summary>
		/// Starts the transaction and returns a disposable handler for it.
		/// </summary>
		/// <param name="action">The transaction.</param>
		public void InTransaction(Action<IDataService> action)
		{
			InTransaction(x =>
			{
				action(x);
				return VoidResult.Instance;
			});
		}

		/// <summary>
		/// Performs the specified operation.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="operation">The operation.</param>
		/// <returns>The result retrieved in the operation.</returns>
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

		/// <summary>
		/// Updates all the stored entities to the state of the specified entities.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			var table = metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type);

			var count = 0;
			foreach (var entity in entities)
			{
				table.Attach(entity, GetEntityById(entity));

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		/// <summary>
		/// Selects the collection of entities of the specified type.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <returns>The result of the query.</returns>
		public IQueryable<TEntity> Select<TEntity>() where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			return metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type).OfType<TEntity>();
		}

		/// <summary>
		/// Selects the entity of the specified type by the specified id.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="id">The id.</param>
		/// <returns>The result of the query.</returns>
		public TEntity SelectById<TEntity>(object id) where TEntity : class
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			var condition = _context.Mapping.GetIdPredicate<TEntity>(id);

			return ((IQueryable<TEntity>)_context.GetTable(typeof(TEntity))).FirstOrDefault(condition);
		}

		/// <summary>
		/// Inserts the specified entities into the underlying data source.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		public int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			var table = metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type);

			var count = 0;
			foreach (var entity in entities)
			{
				table.InsertOnSubmit(entity);

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		/// <summary>
		/// Updates all the specified entities with the specified action.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> action) where TEntity : class
		{
			return Update(entities, (entity, i) => action(entity));
		}

		/// <summary>
		/// Updates all the specified entities with the specified action.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity, int> action) where TEntity : class
		{
			var count = 0;
			foreach (var entity in entities)
			{
				action(entity, count);

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		/// <summary>
		/// Updates all the entities with the specified action matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action) where TEntity : class
		{
			return Update(predicate, (entity, i) => action(entity));
		}

		/// <summary>
		/// Updates all the entities with the specified action matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity, int> action) where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			var table = metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type).OfType<TEntity>();

			var count = 0;
			foreach (var entity in table.Where(predicate))
			{
				action(entity, count);

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		/// <summary>
		/// Removes all the specified entities from the underlying data source.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		public int Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			var table = metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type);

			var count = 0;
			foreach (var entity in entities)
			{
				table.DeleteOnSubmit(entity);

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		/// <summary>
		/// Removes all the entities matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The number of rows affected.</returns>
		public int Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var metadata = _context.Mapping.GetMetaType(typeof(TEntity));

			var table = metadata.InheritanceBase == null ? _context.GetTable<TEntity>() : _context.GetTable(metadata.InheritanceRoot.Type);
			var queryabole = table.OfType<TEntity>();

			var count = 0;
			foreach (var entity in queryabole.Where(predicate))
			{
				table.DeleteOnSubmit(entity);

				count++;
			}

			_context.SubmitChanges();

			return count;
		}

		private TEntity GetEntityById<TEntity>(TEntity source)
		{
			var entities = (IQueryable<TEntity>) _context.GetTable(typeof (TEntity));

			var metaType = _context.Mapping.GetMetaType(typeof (TEntity));
			foreach (var identityMember in metaType.IdentityMembers)
			{
				var x = Expression.Parameter(typeof (TEntity));
				var xMember = Expression.MakeMemberAccess(x, identityMember.Member);
				var sourceMember = Expression.MakeMemberAccess(Expression.Constant(source), identityMember.Member);
				var condition = Expression.Equal(xMember, sourceMember);
				entities = entities.Where(Expression.Lambda<Func<TEntity, bool>>(condition, x));
			}
			return entities.FirstOrDefault();
		}
	}
}