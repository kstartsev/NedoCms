using System;
using System.Data.Linq;
using NedoCms.Data.Interfaces;

namespace NedoCms.Data.Linq
{
	/// <summary>
	/// Represents a data options for data context.
	/// </summary>
	public sealed class DataOptions : IDataOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataOptions"/> class.
		/// </summary>
		/// <param name="initialize">The initialize action for options.</param>
		public DataOptions(Action<DataLoadOptions> initialize)
		{
			if (initialize == null)
			{
				throw new ArgumentNullException("initialize");
			}

			LoadOptions = new DataLoadOptions();

			initialize(LoadOptions);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataOptions"/> class.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		public DataOptions(DataLoadOptions loadOptions)
		{
			LoadOptions = loadOptions;
		}

		/// <summary>
		/// Gets or sets the load options.
		/// </summary>
		/// <value>The load options.</value>
		public DataLoadOptions LoadOptions { get; set; }
	}
}