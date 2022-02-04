using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	internal class Selection : IExpression
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

		public Selection(IList<IExpression> expressions, int start, int end)
		{

			Expressions = expressions;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return string.Join(Constants.VerticalBar, Expressions.Select(e => e.ToString()));
		}
	}
}
