namespace Yagiey.Lib.RegularExpressions.Expressions
{
	internal class Option : IExpression
	{
		public IExpression Expression
		{
			get;
			private set;
		}

		public int Start
		{
			get;
			private set;
		}

		public int End
		{
			get;
			private set;
		}

		public Option(IExpression expr, int start, int end)
		{
			Expression = expr;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			if (Expression is Selection || Expression is Concatenation)
			{
				return Constants.LParen + Expression.ToString() + Constants.RParen + Constants.Question;
			}
			return Expression.ToString() + Constants.Question;
		}
	}
}
