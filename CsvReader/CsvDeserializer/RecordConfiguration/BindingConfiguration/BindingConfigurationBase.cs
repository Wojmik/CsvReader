using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	/// <summary>
	/// Base class for configuring binding property to a csv column
	/// </summary>
	public abstract class BindingConfigurationBase
	{
		/// <summary>
		/// Record configuration object
		/// </summary>
		protected internal WojciechMikołajewicz.CsvReader.RecordConfiguration RecordConfiguration { get; }

		/// <summary>
		/// Name of the column property is bind to
		/// </summary>
		public string? ColumnName { get; private protected set; }

		/// <summary>
		/// Zero based index of the column property is bind to
		/// </summary>
		public int ColumnIndex { get; private protected set; }

		/// <summary>
		/// Is binding to column optional. Default is false which means binding to column is required.
		/// </summary>
		/// <remarks>
		/// For optional bindings no exception is thrown if the column doesn't exist.
		/// </remarks>
		public bool Optional { get; private protected set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="recordConfiguration">Record configuration object</param>
		protected BindingConfigurationBase(WojciechMikołajewicz.CsvReader.RecordConfiguration recordConfiguration)
		{
			RecordConfiguration = recordConfiguration;
			ColumnIndex = -1;
		}

		/// <summary>
		/// Sets property bindings to a column by name
		/// </summary>
		/// <param name="columnName">Name of the column property will be bind to</param>
		protected internal void BindToColumnInternal(string columnName)
		{
			ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
			ColumnIndex = -1;
		}

		/// <summary>
		/// Sets property bindings to a column by zero based column index
		/// </summary>
		/// <param name="columnIndex">Zero based column index property will be bind to</param>
		protected internal void BindToColumnInternal(int columnIndex)
		{
			if(columnIndex<0)
				throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"{nameof(columnIndex)} cannot be less than zero");
			ColumnName = null;
			ColumnIndex = columnIndex;
		}

		/// <summary>
		/// Clears property binding which means property will be ignored during deserialization because lack of binding to a csv column
		/// </summary>
		protected internal void IgnoreInternal()
		{
			ColumnName = null;
			ColumnIndex = -1;
		}

		/// <summary>
		/// Is binding to column optional. No exception is thrown when optional column doesn't exist.
		/// </summary>
		/// <param name="optional">Should binding to column be optional</param>
		protected internal void IsOptionalInternal(bool optional)
		{
			Optional = optional;
		}
	}

	/// <summary>
	/// Base class for configuring binding property to a csv column
	/// </summary>
	/// <typeparam name="TRecord">Type of records read from csv</typeparam>
	public abstract class BindingConfigurationBase<TRecord> : BindingConfigurationBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="recordConfiguration">Record configuration object</param>
		protected BindingConfigurationBase(WojciechMikołajewicz.CsvReader.RecordConfiguration recordConfiguration)
			: base(recordConfiguration)
		{ }

		internal abstract bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out ColumnBinding<TRecord>? columnBinding);
	}
}