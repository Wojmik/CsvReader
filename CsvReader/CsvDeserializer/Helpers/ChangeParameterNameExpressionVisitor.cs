using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.CsvDeserializer.Helpers
{
	class ChangeParameterNameExpressionVisitor : ExpressionVisitor
	{
		public string NewParameterName { get; }

		private ParameterExpression? OldParameter;
		private ParameterExpression? NewParameter;

		public ChangeParameterNameExpressionVisitor(string newParameterName)
		{
			this.NewParameterName = newParameterName;
		}

		public Expression ChangeParameterName(Expression node, ParameterExpression oldParameter, out ParameterExpression newParameter)
		{
			this.OldParameter = oldParameter;
			this.NewParameter = Expression.Parameter(oldParameter.Type, NewParameterName);
			newParameter = this.NewParameter;

			var newExpression = this.Visit(node);

			return newExpression;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			Expression newParameterExpression;

			if(node.Equals(OldParameter))
				newParameterExpression = NewParameter!;
			else
				newParameterExpression = base.VisitParameter(node);

			return newParameterExpression;
		}
	}
}