using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NedoCms.Data.Interfaces
{
	/// <summary>
	/// Represents a basic data provider.
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		IDataOptions Options { get; set; }

		/// <summary>
		/// Selects the collection of entities of the specified type.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <returns>The result of the query.</returns>
		IQueryable<TEntity> Select<TEntity>() where TEntity : class;

		/// <summary>
		/// Selects the entity of the specified type by the specified id.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="id">The id.</param>
		/// <returns>The result of the query.</returns>
		TEntity SelectById<TEntity>(object id) where TEntity : class;

		/// <summary>
		/// Inserts the specified entities into the underlying data source.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

		/// <summary>
		/// Updates all the specified entities
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

		/// <summary>
		/// Updates all the specified entities with the specified action.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> action) where TEntity : class;

		/// <summary>
		/// Updates all the specified entities with the specified action.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		int Update<TEntity>(IEnumerable<TEntity> entities, Action<TEntity, int> action) where TEntity : class;

		/// <summary>
		/// Updates all the entities with the specified action matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action) where TEntity : class;

		/// <summary>
		/// Updates all the entities with the specified action matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <param name="action">The action.</param>
		/// <returns>The number of rows affected.</returns>
		int Update<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity, int> action) where TEntity : class;

		/// <summary>
		/// Removes all the entities matching the specified predicate.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The number of rows affected.</returns>
		int Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

		/// <summary>
		/// Removes all the specified entities from the underlying data source.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entities">The entities.</param>
		/// <returns>The number of rows affected.</returns>
		int Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
	}
}
