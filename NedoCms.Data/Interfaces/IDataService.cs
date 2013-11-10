using System;

namespace NedoCms.Data.Interfaces
{
	/// <summary>
	/// Defines a data layer
	/// </summary>
	public interface IDataService : IDataProvider
	{
		/// <summary>
		/// Starts the transaction and returns a disposable handler for it.
		/// </summary>
		/// <param name="action">The action.</param>
		void InTransaction(Action<IDataService> action);

		/// <summary>
		/// Starts the transaction and returns a disposable handler for it.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="operation">The operation.</param>
		/// <returns>The result of the operations made in the transaction.</returns>
		TResult InTransaction<TResult>(Func<IDataService, TResult> operation);

		/// <summary>
		/// Gets or sets the log.
		/// </summary>
		System.IO.TextWriter Log { get; set; }
	}
}