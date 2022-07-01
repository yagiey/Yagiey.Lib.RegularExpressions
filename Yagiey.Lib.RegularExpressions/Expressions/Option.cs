using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

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

		public Option(IExpression expr, IEnumerator<int> itorID)
		{
			Expression = expr;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public string ToRegularExpression()
		{
			if (Expression is Selection || Expression is Concatenation)
			{
				return Constants.LParen + Expression.ToRegularExpression() + Constants.RParen + Constants.Question;
			}
			return Expression.ToRegularExpression() + Constants.Question;
		}

		public IExpression CopyWithNewNode(IEnumerator<int> itorID)
		{
			IExpression expr = Expression.CopyWithNewNode(itorID);
			return new Option(expr, itorID);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			Expression.AddTransition(transitionMap, itorID);

			NFA.AddTransition(transitionMap, Start, Input.Empty, End);
			NFA.AddTransition(transitionMap, Start, Input.Empty, Expression.Start);
			NFA.AddTransition(transitionMap, Expression.End, Input.Empty, End);
		}
	}
}
