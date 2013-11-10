using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using NedoCms.Data.Interfaces;
using NedoCms.Data.Linq;

namespace NedoCms.Data.Extensions
{
	/// <summary>
	/// Defines useful methods for the DataContext instances.
	/// </summary>
	public static class DataContextExtensions
	{
		public static Expression<Func<TEntity, bool>> GetIdPredicate<TEntity>(this MetaModel model, object id)
		{
			var metaType = model.GetMetaType(typeof(TEntity));
			if (metaType == null || metaType.IdentityMembers.Count == 0)
			{
				throw new InvalidOperationException("The specified entity type doesn't define identifying members.");
			}

			var first = metaType.IdentityMembers[0];
			if (metaType.IdentityMembers.Count == 1 && first.Type == id.GetType())
			{
				var x = Expression.Parameter(typeof(TEntity));
				var xMember = Expression.MakeMemberAccess(x, first.Member);
				return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(xMember, Expression.Constant(id)), x);
			}

			var map = id as IDictionary<string, object>;
			return map != null
				? IdPredicate<TEntity>(metaType, map)
				: IdPredicate<TEntity>(metaType, id);
		}

		private static Expression<Func<TEntity, bool>> IdPredicate<TEntity>(MetaType metaType, IDictionary<string, object> map)
		{
			var x = Expression.Parameter(typeof(TEntity));

			Expression predicate = null;
			foreach (var identityMember in metaType.IdentityMembers)
			{
				var xMember = Expression.MakeMemberAccess(x, identityMember.Member);
				object idMember;
				if (!map.TryGetValue(identityMember.Name, out idMember))
				{
					throw new InvalidOperationException(string.Format("The specified id doesn't contain value for {0} identity member.", identityMember.Name));
				}
				predicate = predicate == null
					? Expression.Equal(xMember, Expression.Constant(idMember))
					: Expression.AndAlso(predicate, Expression.Equal(xMember, Expression.Constant(idMember)));
			} // ReSharper disable AssignNullToNotNullAttribute
			return Expression.Lambda<Func<TEntity, bool>>(predicate, x); // ReSharper restore AssignNullToNotNullAttribute
		}

		private static Expression<Func<TEntity, bool>> IdPredicate<TEntity>(MetaType metaType, object id)
		{
			var x = Expression.Parameter(typeof(TEntity));

			Expression predicate = null;
			foreach (var identityMember in metaType.IdentityMembers)
			{
				var xMember = Expression.MakeMemberAccess(x, identityMember.Member);
				var sourceMember = Expression.PropertyOrField(Expression.Constant(id), identityMember.Name);
				predicate = predicate == null
					? Expression.Equal(xMember, sourceMember)
					: Expression.AndAlso(predicate, Expression.Equal(xMember, sourceMember));
			} // ReSharper disable AssignNullToNotNullAttribute
			return Expression.Lambda<Func<TEntity, bool>>(predicate, x); // ReSharper restore AssignNullToNotNullAttribute
		}
	}
}
