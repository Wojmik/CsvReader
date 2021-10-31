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
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;
using WojciechMikołajewicz.CsvReader.CsvNodes;
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
		private IEqualityComparer<string> HeaderRowColumnNamesComparer { get; }

		/// <summary>
		/// Check if columns count is equal in every row
		/// </summary>
		private readonly bool CheckColumnsCountConsistency;

		/// <summary>
		/// Low level csv reader
		/// </summary>
		private CsvReader CsvReader { get; }

		private StringDeduplicator StringDeduplicator { get; }

		private ColumnBinding<TRecord>[] ColumnBinders;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textReader">Text reader</param>
		/// <param name="options">Options</param>
		/// <param name="recordConfiguration"><typeparamref name="TRecord"/> type configuration</param>
		public CsvDeserializer(TextReader textReader, CsvDeserializerOptions? options = null, ICsvRecordTypeConfiguration<TRecord>? recordConfiguration = null)
			: this(options!=null ? new CsvReader(textReader, options) : new CsvReader(textReader), options, recordConfiguration)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="csvReader">Low level csv reader</param>
		/// <param name="options">Options</param>
		/// <param name="recordConfiguration"><typeparamref name="TRecord"/> type configuration</param>
		public CsvDeserializer(CsvReader csvReader, CsvDeserializerOptions? options = null, ICsvRecordTypeConfiguration<TRecord>? recordConfiguration = null)
		{
			if(options==null)
				options = new CsvDeserializerOptions();

			HasHeaderRow = options.HasHeaderRow;
			HeaderRowColumnNamesComparer = options.HeaderRowColumnNamesComparer;
			CheckColumnsCountConsistency = options.CheckColumnsCountConsistency;

			CsvReader = csvReader??throw new ArgumentNullException(nameof(csvReader));

			StringDeduplicator = StringDeduplicator.Create();

			//TRecord type configuration
			var recordConfigurationBuilder = new RecordConfiguration<TRecord>(options, StringDeduplicator);
			//Automatic discover properties of TRecord and add bindings
			recordConfigurationBuilder.DiscoverRecordBinding();
			//Set configuration provided by user
			if(recordConfiguration!=null)
				recordConfiguration.Configure(recordConfigurationBuilder);

			var columnBinders = recordConfigurationBuilder.Build().ToArray();
			if(!options.HasHeaderRow)
				SetBindingOrder(columnBinders);
			ColumnBinders = columnBinders;
		}

		private void SetBindingOrder(ColumnBinding<TRecord>[] columnBindings)
		{
			//Sort array by ColumnIndex
			Array.Sort(columnBindings, Comparison);

			if(0<columnBindings.Length)
			{
				//All column indexes have to be valid
				if(columnBindings[0].ColumnIndex<0)
					throw new InvalidOperationException($"Could not find out column index for column name: \"{columnBindings[0].ColumnName}\"");
			}

			static int Comparison(ColumnBinding<TRecord> x, ColumnBinding<TRecord> y)
			{
				int cmp;

				if(0==(cmp=x.ColumnIndex.CompareTo(y.ColumnIndex)))//Same order as CsvReader will yield cells
					cmp=x.InputType.CompareTo(y.InputType);//This order is very important - first should go memory sequences, then memories, strings last

				return cmp;
			}
		}

		private async ValueTask<int> ReadHeaderRowAsync(CancellationToken cancellationToken)
		{
			var columnsByName = ColumnBinders.Where(bnd => bnd.ColumnName!=null).ToLookup(bnd => bnd.ColumnName!, HeaderRowColumnNamesComparer);
			var columnsByIndex = ColumnBinders.Where(bnd => bnd.ColumnName==null).ToLookup(bnd => bnd.ColumnIndex);

			StringNode node;
			int columnIndex = 0;
			var bindings = new List<ColumnBinding<TRecord>>(ColumnBinders.Length);
			while((node = await CsvReader.ReadNextNodeAsStringAsync(cancellationToken).ConfigureAwait(false)).NodeType==NodeType.Cell)
			{
				foreach(var binding in columnsByName[node.Data])
				{
					binding.ColumnIndex = columnIndex;
					bindings.Add(binding);
				}
				bindings.AddRange(columnsByIndex[columnIndex]);

				columnIndex++;
			}

			var columnBinders = bindings.ToArray();
			SetBindingOrder(columnBinders);
			ColumnBinders = columnBinders;

			return columnIndex;
		}

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
		/// <summary>
		/// Enumerates records from csv
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Enumerable of <typeparamref name="TRecord"/></returns>
		/// <exception cref="System.Runtime.Serialization.SerializationException">
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

			if(HasHeaderRow)
			{
				columnsCount = await ReadHeaderRowAsync(cancellationToken)
					.ConfigureAwait(false);
				rowIndex++;
			}

			while(true)
			{
				ProcessCellParams<TRecord> data = new ProcessCellParams<TRecord>(new TRecord());
				MemorySequenceNode node;

				while((node=await CsvReader.ReadNextNodeAsMemorySequenceAsync(cancellationToken).ConfigureAwait(false)).NodeType==NodeType.Cell)
				{
					try
					{
						ProcessCell(node.MemorySequence, ref data);
					}
					catch(Exception ex)
					{
						throw new SerializationException($"Error parsing data in row {rowIndex} and column index {data.ColumnIndex}, name \"{ColumnBinders[data.BinderIndex].ColumnName}\"", ex);
					}
				}

				//If end of stream detected and didn't read any column then break
				if(node.NodeType!=NodeType.NewLine && data.ColumnIndex==0)
					break;

				//Check columns count consistency
				if(CheckColumnsCountConsistency && data.ColumnIndex!=columnsCount)
				{
					if(0<=columnsCount)
						throw new SerializationException($"Inconsistent number of columns in row {rowIndex}. Should be {columnsCount} columns and is {data.ColumnIndex}");
					columnsCount = data.ColumnIndex;
				}

				yield return data.Record;

				rowIndex++;
			}
		}
#endif

		/// <summary>
		/// Enumerates records from csv
		/// </summary>
		/// <returns>Enumerable of <typeparamref name="TRecord"/></returns>
		/// <exception cref="System.Runtime.Serialization.SerializationException">
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

			if(HasHeaderRow)
			{
				columnsCount = ReadHeaderRowAsync(default).GetAwaiter().GetResult();
				rowIndex++;
			}

			while(true)
			{
				ProcessCellParams<TRecord> data = new ProcessCellParams<TRecord>(new TRecord());
				MemorySequenceNode node;

				while((node=CsvReader.ReadNextNodeAsMemorySequenceAsync().GetAwaiter().GetResult()).NodeType==NodeType.Cell)
				{
					try
					{
						ProcessCell(node.MemorySequence, ref data);
					}
					catch(Exception ex)
					{
						throw new SerializationException($"Error parsing data in row {rowIndex} and column index {data.ColumnIndex}, name \"{ColumnBinders[data.BinderIndex].ColumnName}\"", ex);
					}
				}

				//If end of stream detected and didn't read any column then break
				if(node.NodeType!=NodeType.NewLine && data.ColumnIndex==0)
					break;

				//Check columns count consistency
				if(CheckColumnsCountConsistency && data.ColumnIndex!=columnsCount)
				{
					if(0<=columnsCount)
						throw new SerializationException($"Inconsistent number of columns in row {rowIndex}. Should be {columnsCount} columns and is {data.ColumnIndex}");
					columnsCount = data.ColumnIndex;
				}

				yield return data.Record;

				rowIndex++;
			}
		}

		private void ProcessCell(in MemorySequenceSpan memorySequence, ref ProcessCellParams<TRecord> parameters)
		{
			ColumnBinding<TRecord> binding;
			NodeContainer nodeContainer = default;

			while(parameters.BinderIndex<ColumnBinders.Length && (binding=ColumnBinders[parameters.BinderIndex]).ColumnIndex==parameters.ColumnIndex)
			{
				if(nodeContainer.NodeContainerType<binding.InputType)
					switch(binding.InputType)
					{
						case NodeContainerType.MemorySequence:
							nodeContainer = new NodeContainer(memorySequence);
							break;
						case NodeContainerType.Memory:
							var memory = CsvReader.MemorySequenceAsMemory(memorySequence);
							nodeContainer = new NodeContainer(memory);
							break;
						case NodeContainerType.String:
							string str;
							if(nodeContainer.NodeContainerType==NodeContainerType.Memory)
								str = nodeContainer.Memory.ToString();
							else
								str = CsvReader.MemorySequenceToString(memorySequence);
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

		/// <summary>
		/// Disposes <see cref="CsvDeserializer{TRecord}"/>
		/// </summary>
		public void Dispose()
		{
			this.CsvReader.Dispose();
		}
	}
}