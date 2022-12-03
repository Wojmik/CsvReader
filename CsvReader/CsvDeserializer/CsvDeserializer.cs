using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WojciechMikołajewicz.CsvReader.CsvDeserializer;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordSetter;
using WojciechMikołajewicz.CsvReader.CsvNodes;
using WojciechMikołajewicz.CsvReader.Exceptions;
using WojciechMikołajewicz.CsvReader.Helpers;
using WojciechMikołajewicz.CsvReader.InternalModel;

namespace WojciechMikołajewicz.CsvReader
{
	/// <summary>
	/// Csv deserializer to <typeparamref name="TRecord"/> objects
	/// </summary>
	/// <typeparam name="TRecord">Type of csv record (row)</typeparam>
	public class CsvDeserializer<TRecord> : IDisposable
		where TRecord : class, new()
	{
		/// <summary>
		/// Does csv contain header row
		/// </summary>
		public bool HasHeaderRow { get; }

		/// <summary>
		/// Comparer of column names in header row
		/// </summary>
		private readonly IEqualityComparer<string> _headerRowColumnNamesComparer;

		/// <summary>
		/// Check if columns count is equal in every row
		/// </summary>
		private readonly bool _checkColumnsCountConsistency;

		private int _numberOfRequiredColumns;

		/// <summary>
		/// Low level csv reader
		/// </summary>
		private readonly CsvReader _csvReader;

		private readonly StringDeduplicator _stringDeduplicator;

		private ColumnBinding<TRecord>[] _columnBinders;

		private static CsvDeserializerOptions CreateOptions(Action<ICsvReaderAndDeserializerOptions>? optionsDelegate)
		{
			var options = new CsvDeserializerOptions();
			optionsDelegate?.Invoke(options);
			return options;
		}

		private static CsvReaderAndOptions CreateCsvReaderAndOptions(TextReader textReader, Action<ICsvReaderAndDeserializerOptions>? optionsDelegate)
		{
			var options = CreateOptions(optionsDelegate);
			var csvReader = new CsvReader(textReader, options);
			return new CsvReaderAndOptions(csvReader, options);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textReader">Text reader</param>
		/// <param name="optionsDelegate">Method to configure options</param>
		/// <param name="recordConfiguration"><typeparamref name="TRecord"/> type configuration</param>
		/// <exception cref="ArgumentNullException"><paramref name="textReader"/> is null or <paramref name="recordConfiguration"/> is null with lack of header row</exception>
		/// <exception cref="BindingConfigurationException">Binding by column names with lack of header row or optional columns bound by index before required columns detected</exception>
		public CsvDeserializer(TextReader textReader, Action<ICsvReaderAndDeserializerOptions>? optionsDelegate = null, ICsvRecordTypeConfiguration<TRecord>? recordConfiguration = null)
			: this(CreateCsvReaderAndOptions(textReader, optionsDelegate), recordConfiguration)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="csvReader">Low level csv reader</param>
		/// <param name="optionsDelegate">Method to configure options</param>
		/// <param name="recordConfiguration"><typeparamref name="TRecord"/> type configuration</param>
		/// <exception cref="ArgumentNullException"><paramref name="csvReader"/> is null or <paramref name="recordConfiguration"/> is null with lack of header row</exception>
		/// <exception cref="BindingConfigurationException">Binding by column names with lack of header row or optional columns bound by index before required columns detected</exception>
		public CsvDeserializer(CsvReader csvReader, Action<ICsvDeserializerOptions>? optionsDelegate = null, ICsvRecordTypeConfiguration<TRecord>? recordConfiguration = null)
			: this(new CsvReaderAndOptions(csvReader, CreateOptions(optionsDelegate)), recordConfiguration)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="csvReaderAndOptions">Low level csv reader and options</param>
		/// <param name="recordConfiguration"><typeparamref name="TRecord"/> type configuration</param>
		/// <exception cref="ArgumentNullException">csvReaderAndOptions.csvReader is null or csvReaderAndOptions.options is null or <paramref name="recordConfiguration"/> is null with lack of header row</exception>
		/// <exception cref="BindingConfigurationException">Binding by column names with lack of header row or optional columns bound by index before required columns detected</exception>
		private CsvDeserializer(CsvReaderAndOptions csvReaderAndOptions, ICsvRecordTypeConfiguration<TRecord>? recordConfiguration)
		{
			var (csvReader, options) = csvReaderAndOptions;

			_csvReader = csvReader ?? throw new ArgumentNullException(nameof(csvReader));

			if (options == null)
				throw new ArgumentNullException(nameof(options));
			HasHeaderRow = options.HasHeaderRow;
			_headerRowColumnNamesComparer = options.HeaderRowColumnNamesComparer;
			_checkColumnsCountConsistency = options.CheckColumnsCountConsistency;

			_stringDeduplicator = StringDeduplicator.Create();

			//TRecord type configuration
			var recordConfigurationBuilder = new RecordConfiguration<TRecord>(options, _stringDeduplicator);
			//Automatic discover properties of TRecord and add bindings
			recordConfigurationBuilder.DiscoverRecordBinding();
			//Set configuration provided by user
			if (recordConfiguration != null)
				recordConfiguration.Configure(recordConfigurationBuilder);
			else if (!HasHeaderRow)
				throw new ArgumentNullException(nameof(recordConfiguration), $"{nameof(recordConfiguration)} is required when there is no header row");

			var columnBinders = recordConfigurationBuilder.Build().ToArray();
			//Check bindings
			bool optionalColumns = false, bindingsByName = false;
			foreach (var columnBinder in columnBinders)
				switch (columnBinder.Type)
				{
					case BindingType.ByColumnIndex:
						if (columnBinder.Required)
						{
							if (optionalColumns)
								throw new BindingConfigurationException("Optional columns bound by index can only exist after required columns");
						}
						else
							optionalColumns = true;
						break;
					case BindingType.ByColumnName:
						bindingsByName = true;
						break;
					default:
						throw new NotSupportedException($"{typeof(BindingType)} has not supported value: {columnBinder.Type}");
				}

			if (!HasHeaderRow)
			{
				if (bindingsByName)
					throw new BindingConfigurationException("Binding by column names require header row");
				_numberOfRequiredColumns = SetBindingOrder(columnBinders);
			}
			_columnBinders = columnBinders;
		}

		private int SetBindingOrder(ColumnBinding<TRecord>[] columnBindings)
		{
			//Sort array by ColumnIndex
			Array.Sort(columnBindings, (x, y) =>
			{
				int cmp;

				if (0 == (cmp = x.ColumnIndex.CompareTo(y.ColumnIndex)))//Same order as CsvReader will yield cells
					cmp = x.InputType.CompareTo(y.InputType);//This order is very important - first should go memory sequences, then memories, strings last

				return cmp;
			});

			//Check the number of required columns
			int numberOfRequiredColumns = 0;
			for (var i = columnBindings.Length - 1; i >= 0; i--)
				if (columnBindings[i].Required)
				{
					numberOfRequiredColumns = columnBindings[i].ColumnIndex + 1;
					break;
				}
			return numberOfRequiredColumns;
		}

		private async ValueTask<int> ReadHeaderRowAsync(CancellationToken cancellationToken)
		{
			var columnsByName = _columnBinders.Where(bnd => bnd.ColumnName != null).ToLookup(bnd => bnd.ColumnName!, _headerRowColumnNamesComparer);
			var columnsByIndex = _columnBinders.Where(bnd => bnd.ColumnName == null && bnd.ColumnIndex >= 0).ToLookup(bnd => bnd.ColumnIndex);

			StringNode node;
			int columnIndex = 0;
			var bindings = new List<ColumnBinding<TRecord>>(_columnBinders.Length);
			while ((node = await _csvReader.ReadNextNodeAsStringAsync(cancellationToken).ConfigureAwait(false)).NodeType == NodeType.Cell)
			{
				foreach (var binding in columnsByName[node.Data])
				{
					binding.ColumnIndex = columnIndex;
					bindings.Add(binding);
				}
				bindings.AddRange(columnsByIndex[columnIndex]);

				columnIndex++;
			}

			//Check if all required properties were binded to columns
			List<MissingBindingsException.ColumnInfo>? missingColumns = null;
			foreach (var binding in _columnBinders)
			{
				if (!binding.Required)
					continue;

				if (binding.ColumnIndex < 0 && binding.ColumnName != null)
				{
					if (missingColumns == null)
						missingColumns = new List<MissingBindingsException.ColumnInfo>();
					missingColumns.Add(new MissingBindingsException.ColumnInfo(binding.ColumnName, binding.ColumnIndex));
				}
				else if (binding.ColumnIndex >= columnIndex)
				{
					if (missingColumns == null)
						missingColumns = new List<MissingBindingsException.ColumnInfo>();
					missingColumns.Add(new MissingBindingsException.ColumnInfo(binding.ColumnName, binding.ColumnIndex));
				}
			}
			if (missingColumns != null)
				throw new MissingBindingsException(missingColumns, columnIndex);

			var columnBinders = bindings.ToArray();
			_numberOfRequiredColumns = SetBindingOrder(columnBinders);
			_columnBinders = columnBinders;

			return columnIndex;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
		/// <summary>
		/// Enumerates records from csv
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Enumerable of <typeparamref name="TRecord"/></returns>
		/// <exception cref="DeserializationException">
		/// <para>Csv not well formated.</para>
		/// <list type="bullet">
		/// <item>Unexpected character after escaped cell</item>
		/// <item>Unexpected end of stream inside escaped cell</item>
		/// <item>Inconsistent number of columns in rows when <see cref="CsvDeserializerOptions.CheckColumnsCountConsistency"/> set to true</item>
		/// </list>
		/// </exception>
		public async IAsyncEnumerable<TRecord> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			int columnsCount = -1, rowIndex = 0;

			if (HasHeaderRow)
			{
				columnsCount = await ReadHeaderRowAsync(cancellationToken)
					.ConfigureAwait(false);
				rowIndex++;
			}

			while (true)
			{
				var data = new ProcessCellParams<TRecord>(new TRecord());
				MemorySequenceNode node;

				while ((node = await _csvReader.ReadNextNodeAsMemorySequenceAsync(cancellationToken).ConfigureAwait(false)).NodeType == NodeType.Cell)
				{
					try
					{
						ProcessCell(node.MemorySequence, ref data);
					}
					catch (Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Error parsing data in row {rowIndex} and column index {data.ColumnIndex}, name \"{_columnBinders[data.BinderIndex].ColumnName}\"", ex);
					}
				}

				//If end of stream detected and didn't read any column then break
				if (node.NodeType != NodeType.NewLine && data.ColumnIndex == 0)
					break;

				//Check columns count consistency
				if (data.ColumnIndex != columnsCount)
				{
					if(_checkColumnsCountConsistency && 0 <= columnsCount)
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Inconsistent number of columns in row {rowIndex}. Should be {columnsCount} columns and is {data.ColumnIndex}");
					columnsCount = data.ColumnIndex;
					try
					{
						CheckRequiredBindings(columnsCount);
					}
					catch (Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Too less columns to deserialize all required properties. The number of required columns is {_numberOfRequiredColumns} and the actual number of columns id {data.ColumnIndex}", ex);
					}
				}

				yield return data.Record;

				rowIndex++;
			}
		}
#endif

		/// <summary>
		/// Reads all records from csv to a <see cref="List{T}"/>
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of <typeparamref name="TRecord"/></returns>
		/// <exception cref="DeserializationException">
		/// <para>Csv not well formated.</para>
		/// <list type="bullet">
		/// <item>Unexpected character after escaped cell</item>
		/// <item>Unexpected end of stream inside escaped cell</item>
		/// <item>Inconsistent number of columns in rows when <see cref="CsvDeserializerOptions.CheckColumnsCountConsistency"/> set to true</item>
		/// </list>
		/// </exception>
		public async Task<List<TRecord>> ReadAllToListAsync(CancellationToken cancellationToken = default)
		{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			var records = new List<TRecord>();

			await foreach (var record in ReadAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
			{
				records.Add(record);
			}
			return records;
#else
			int columnsCount = -1, rowIndex = 0;

			if (HasHeaderRow)
			{
				columnsCount = await ReadHeaderRowAsync(cancellationToken)
					.ConfigureAwait(false);
				rowIndex++;
			}

			var records = new List<TRecord>();
			while (true)
			{
				ProcessCellParams<TRecord> data = new ProcessCellParams<TRecord>(new TRecord());
				MemorySequenceNode node;

				while ((node = await _csvReader.ReadNextNodeAsMemorySequenceAsync(cancellationToken).ConfigureAwait(false)).NodeType == NodeType.Cell)
				{
					try
					{
						ProcessCell(node.MemorySequence, ref data);
					}
					catch (Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Error parsing data in row {rowIndex} and column index {data.ColumnIndex}, name \"{_columnBinders[data.BinderIndex].ColumnName}\"", ex);
					}
				}

				//If end of stream detected and didn't read any column then break
				if (node.NodeType != NodeType.NewLine && data.ColumnIndex == 0)
					break;

				//Check columns count consistency
				if (data.ColumnIndex != columnsCount)
				{
					if (_checkColumnsCountConsistency && 0 <= columnsCount)
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Inconsistent number of columns in row {rowIndex}. Should be {columnsCount} columns and is {data.ColumnIndex}");
					columnsCount = data.ColumnIndex;
					try
					{
						CheckRequiredBindings(columnsCount);
					}
					catch (Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Too less columns to deserialize all required properties. The number of required columns is {_numberOfRequiredColumns} and the actual number of columns id {data.ColumnIndex}", ex);
					}
				}

				records.Add(data.Record);

				rowIndex++;
			}

			return records;
#endif
		}

		/// <summary>
		/// Enumerates records from csv
		/// </summary>
		/// <returns>Enumerable of <typeparamref name="TRecord"/></returns>
		/// <exception cref="DeserializationException">
		/// <para>Csv not well formated.</para>
		/// <list type="bullet">
		/// <item>Unexpected character after escaped cell</item>
		/// <item>Unexpected end of stream inside escaped cell</item>
		/// <item>Inconsistent number of columns in rows when <see cref="CsvDeserializerOptions.CheckColumnsCountConsistency"/> set to true</item>
		/// </list>
		/// </exception>
		public IEnumerable<TRecord> Read()
		{
			int columnsCount = -1, rowIndex = 0;

			if (HasHeaderRow)
			{
				columnsCount = ReadHeaderRowAsync(default).GetAwaiter().GetResult();
				rowIndex++;
			}

			while (true)
			{
				ProcessCellParams<TRecord> data = new ProcessCellParams<TRecord>(new TRecord());
				MemorySequenceNode node;

				while ((node = _csvReader.ReadNextNodeAsMemorySequenceAsync().GetAwaiter().GetResult()).NodeType == NodeType.Cell)
				{
					try
					{
						ProcessCell(node.MemorySequence, ref data);
					}
					catch (Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Error parsing data in row {rowIndex} and column index {data.ColumnIndex}, name \"{_columnBinders[data.BinderIndex].ColumnName}\"", ex);
					}
				}

				//If end of stream detected and didn't read any column then break
				if (node.NodeType != NodeType.NewLine && data.ColumnIndex == 0)
					break;

				//Check columns count consistency
				if (data.ColumnIndex != columnsCount)
				{
					if (_checkColumnsCountConsistency && 0 <= columnsCount)
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Inconsistent number of columns in row {rowIndex}. Should be {columnsCount} columns and is {data.ColumnIndex}");
					columnsCount = data.ColumnIndex;
					try
					{
						CheckRequiredBindings(columnsCount);
					}
					catch(Exception ex)
					{
						throw new DeserializationException(rowIndex, data.ColumnIndex, _columnBinders[data.BinderIndex].ColumnName, $"Too less columns to deserialize all required properties. The number of required columns is {_numberOfRequiredColumns} and the actual number of columns id {data.ColumnIndex}", ex);
					}
				}

				yield return data.Record;

				rowIndex++;
			}
		}

		/// <summary>
		/// Reads all records from csv to a <see cref="List{T}"/>
		/// </summary>
		/// <returns>List of <typeparamref name="TRecord"/></returns>
		/// <exception cref="DeserializationException">
		/// <para>Csv not well formated.</para>
		/// <list type="bullet">
		/// <item>Unexpected character after escaped cell</item>
		/// <item>Unexpected end of stream inside escaped cell</item>
		/// <item>Inconsistent number of columns in rows when <see cref="CsvDeserializerOptions.CheckColumnsCountConsistency"/> set to true</item>
		/// </list>
		/// </exception>
		public List<TRecord> ReadAllToList()
		{
			var records = new List<TRecord>();

			foreach(var record in Read())
			{
				records.Add(record);
			}
			return records;
		}

		private void ProcessCell(in MemorySequenceSpan memorySequence, ref ProcessCellParams<TRecord> parameters)
		{
			ColumnBinding<TRecord> binding;
			NodeContainer nodeContainer = default;

			while (parameters.BinderIndex < _columnBinders.Length && (binding = _columnBinders[parameters.BinderIndex]).ColumnIndex == parameters.ColumnIndex)
			{
				if (nodeContainer.NodeContainerType < binding.InputType)
					switch (binding.InputType)
					{
						case NodeContainerType.MemorySequence:
							nodeContainer = new NodeContainer(memorySequence);
							break;
						case NodeContainerType.Memory:
							var memory = _csvReader.MemorySequenceAsMemory(memorySequence);
							nodeContainer = new NodeContainer(memory);
							break;
						case NodeContainerType.String:
							string str;
							if (nodeContainer.NodeContainerType == NodeContainerType.Memory)
								str = nodeContainer.Memory.ToString();
							else
								str = _csvReader.MemorySequenceToString(memorySequence);
							nodeContainer = new NodeContainer(str);
							break;
						default:
							throw new InvalidOperationException($"Unsupported deserializer input type: {binding.InputType}");
					}

				binding.Deserialize(parameters.Record, nodeContainer);

				parameters.BinderIndex++;
			}

			parameters.ColumnIndex++;
		}

		private void CheckRequiredBindings(int columnsCount)
		{
			if (_numberOfRequiredColumns <= columnsCount)
				return;

			var missingColumns = new List<MissingBindingsException.ColumnInfo>();
			for(var i = _columnBinders.Length - 1; i >= 0; i--)
			{
				var columnBinder = _columnBinders[i];

				if (columnBinder.ColumnIndex < columnsCount)
					break;
				missingColumns.Add(new MissingBindingsException.ColumnInfo(columnBinder.ColumnName, columnBinder.ColumnIndex));
			}
			if (missingColumns.Count <= 0)
				return;
			
			missingColumns.Reverse();
			throw new MissingBindingsException(missingColumns, columnsCount);
		}

		/// <summary>
		/// Disposes <see cref="CsvDeserializer{TRecord}"/>
		/// </summary>
		public void Dispose()
		{
			_csvReader.Dispose();
		}
	}
}