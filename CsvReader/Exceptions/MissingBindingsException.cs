using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.Exceptions
{
	/// <summary>
	/// Missing binding exception
	/// </summary>
	public class MissingBindingsException : Exception
	{
		/// <summary>
		/// Collection of missing columns
		/// </summary>
		public IReadOnlyList<ColumnInfo> MissingColumns { get; }

		/// <summary>
		/// The number of columns
		/// </summary>
		public int ColumnsCount { get; }

		private readonly string _message;
		/// <summary>
		/// Error message
		/// </summary>
		public override string Message { get => _message; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="missingColumns">Collection of missing colums</param>
		/// <param name="columnsCount">The number of columns</param>
		/// <exception cref="ArgumentNullException"><paramref name="missingColumns"/> is null</exception>
		/// <exception cref="ArgumentException"><paramref name="missingColumns"/> is empty or <paramref name="columnsCount"/> is less than zero</exception>
		public MissingBindingsException(List<ColumnInfo> missingColumns, int columnsCount)
		{
			if (missingColumns == null)
				throw new ArgumentNullException(nameof(missingColumns));
			if (missingColumns.Count <= 0)
				throw new ArgumentException($"{nameof(missingColumns)} cannot be empty", nameof(missingColumns));
			MissingColumns = missingColumns;
			
			if (columnsCount < 0)
				throw new ArgumentException($"{nameof(columnsCount)} cannot be less than zero and is {columnsCount}", nameof(columnsCount));
			ColumnsCount = columnsCount;

			_message = $"Missing columns for non optional property bindings: {string.Join(", ", MissingColumns)}. The number of columns is {ColumnsCount}.";
		}

		/// <summary>
		/// Column info
		/// </summary>
		public readonly struct ColumnInfo
		{
			/// <summary>
			/// Column name
			/// </summary>
			public string? Name { get; }

			/// <summary>
			/// Column index
			/// </summary>
			public int? Index { get; }

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="name">Column name</param>
			/// <param name="index">Column index</param>
			/// <exception cref="InvalidOperationException"><paramref name="name"/> and <paramref name="index"/> are null simultaneously</exception>
			public ColumnInfo(string? name, int? index)
			{
				if (name == null && !index.HasValue)
					throw new InvalidOperationException($"{nameof(name)} and {nameof(index)} cannot be null simultaneously");
				Name = name;
				Index = index;
			}

			internal ColumnInfo(string? name, int index)
				: this(name, index >= 0 ? (int?)index : null)
			{ }

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override string ToString()
			{
				if (Name != null && Index.HasValue)
					return $"(name: {Name} index: {Index.Value})";
				else if (Name != null)
					return $"(name: {Name})";
				else if (Index.HasValue)
					return $"(index: {Index.Value})";
				else
					return "()";
			}
		}
	}
}