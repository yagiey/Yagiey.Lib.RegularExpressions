using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class Selection : IExpression
	{
		public IEnumerable<IExpression> Expressions
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

		public Selection(IEnumerable<IExpression> expressions, IEnumerator<int> itorID)
		{
			Expressions = expressions;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public string ToRegularExpression()
		{
			return string.Join(Constants.VerticalBar, Expressions.Select(e => e.ToRegularExpression()));
		}

		public IExpression CopyWithNewNode(IEnumerator<int> itorID)
		{
			List<IExpression> exprs = new();
			foreach (IExpression expr in Expressions)
			{
				exprs.Add(
					expr.CopyWithNewNode(itorID)
				);
			}
			return new Selection(exprs, itorID);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			foreach (IExpression expr in Expressions)
			{
				expr.AddTransition(transitionMap, itorID);
				NFA.AddTransition(transitionMap, Start, Input.Empty, expr.Start);
				NFA.AddTransition(transitionMap, expr.End, Input.Empty, End);
			}
		}
	}
}
