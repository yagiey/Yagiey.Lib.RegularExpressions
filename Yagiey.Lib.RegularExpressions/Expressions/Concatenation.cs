using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	internal class Concatenation : IExpression
	{
		public IList<IExpression> Expressions
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

		public Concatenation(IList<IExpression> expressions, int start, int end)
		{
			Expressions = expressions;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			var result = string.Join("", Expressions.Select(e => e is Selection ? Constants.LParen + e.ToString() + Constants.RParen : e.ToString()));
			return result;
		}
	}
}
