using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.CellDeserializers;
using WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.ColumnBinders;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.RecordConfiguration.BindingConfiguration
{
	public abstract class PropertyConfigurationBase<TRecord, TProperty> : BindingConfigurationBase<TRecord>
	{
		protected internal Expression<Func<TRecord, TProperty>> PropertySelector { get; }

		protected PropertyConfigurationBase(RecordConfiguration<TRecord> recordConfiguration, Expression<Func<TRecord, TProperty>> selector)
			: base(recordConfiguration)
		{
			PropertySelector = selector;
		}

		protected Action<TRecord, TProperty> GetSetPropertyDelegate()
		{
			var exprParameter = Expression.Parameter(PropertySelector.Body.Type);
			var exprSetter = Expression.Lambda<Action<TRecord, TProperty>>(Expression.Assign(PropertySelector.Body, exprParameter), PropertySelector.Parameters[0], exprParameter);

			var setter = exprSetter.Compile();
			return setter;
		}

		internal sealed override bool TryBuild(
#if NETSTANDARD2_1_OR_GREATER
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
			columnBinding = new ColumnBindingToProperty<TRecord, TProperty>(ColumnName, ColumnIndex, cellDeserializer!, setPropertyDelegate);
			return true;
		}

		protected abstract bool TryBuildDeserializer(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
# endif
			out CellDeserializerBase<TProperty>? cellDeserializer);
	}
}