using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.Helpers
{
	class ChangeParameterNameExpressionVisitor : ExpressionVisitor
	{
		public string NewParameterName { get; }

		private ParameterExpression? _oldParameter;
		private ParameterExpression? _newParameter;

		public ChangeParameterNameExpressionVisitor(string newParameterName)
		{
			NewParameterName = newParameterName;
		}

		public Expression ChangeParameterName(Expression node, ParameterExpression oldParameter, out ParameterExpression newParameter)
		{
			_oldParameter = oldParameter;
			_newParameter = Expression.Parameter(oldParameter.Type, NewParameterName);
			newParameter = _newParameter;

			var newExpression = Visit(node);

			return newExpression;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			Expression newParameterExpression;

			if(node.Equals(_oldParameter))
				newParameterExpression = _newParameter!;
			else
				newParameterExpression = base.VisitParameter(node);

			return newParameterExpression;
		}
	}
}