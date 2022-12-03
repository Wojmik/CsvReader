using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	/// <summary>
	/// Base class for configuring property
	/// </summary>
	/// <typeparam name="TRecord">Type of records read from csv</typeparam>
	/// <typeparam name="TProperty">Type of record's property</typeparam>
	public abstract class PropertyConfigurationBase<TRecord, TProperty> : BindingConfigurationBase<TRecord>
	{
		/// <summary>
		/// Expression pointing to the record's property
		/// </summary>
		protected internal Expression<Func<TRecord, TProperty>> PropertySelector { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="recordConfiguration">Record configuration object</param>
		/// <param name="selector">Expression pointing to the record's property</param>
		protected PropertyConfigurationBase(WojciechMikołajewicz.CsvReader.RecordConfiguration recordConfiguration, Expression<Func<TRecord, TProperty>> selector)
			: base(recordConfiguration)
		{
			PropertySelector = selector;
		}

		internal void ChangePropertySelector(Expression<Func<TRecord, TProperty>> propertySelector)
		{
			PropertySelector = propertySelector;
		}

		/// <summary>
		/// Returns a delegate storing value to a record property pointed by <see cref="PropertySelector"/>
		/// </summary>
		/// <returns>Delegate that stores value to a record property pointed by <see cref="PropertySelector"/></returns>
		protected Action<TRecord, TProperty> GetSetPropertyDelegate()
		{
			var exprParameter = Expression.Parameter(PropertySelector.Body.Type);
			var exprSetter = Expression.Lambda<Action<TRecord, TProperty>>(Expression.Assign(PropertySelector.Body, exprParameter), PropertySelector.Parameters[0], exprParameter);

			var setter = exprSetter.Compile();
			return setter;
		}

		internal sealed override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
#endif
			out ColumnBinding<TRecord>? columnBinding)
		{
			if((ColumnName==null && ColumnIndex<0) || !TryBuildDeserializer(out var cellDeserializer))
			{
				columnBinding = null;
				return false;
			}

			var setPropertyDelegate = GetSetPropertyDelegate();
			columnBinding = new ColumnBindingToProperty<TRecord, TProperty>(ColumnName, ColumnIndex, Optional, cellDeserializer!, setPropertyDelegate);
			return true;
		}

		/// <summary>
		/// Tries build cell deserializer to property's type
		/// </summary>
		/// <param name="cellDeserializer">Cell deserializer</param>
		/// <returns>Did building succeed</returns>
		protected abstract bool TryBuildDeserializer(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TProperty>? cellDeserializer);
	}
}