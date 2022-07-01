using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class Repeat0 : IExpression
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

		public int Node1
		{
			get;
			private set;
		}

		public int Node2
		{
			get;
			private set;
		}

		public Repeat0(IExpression expr, IEnumerator<int> itorID)
		{
			Expression = expr;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			Node1 = itorID.Current;
			itorID.MoveNext();
			Node2 = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public string ToRegularExpression()
		{
			if (Expression is Selection || Expression is Concatenation)
			{
				return Constants.LParen + Expression.ToRegularExpression() + Constants.RParen + Constants.Asterisk;
			}
			return Expression.ToRegularExpression() + Constants.Asterisk;
		}

		public IExpression CopyWithNewNode(IEnumerator<int> itorID)
		{
			IExpression expr = Expression.CopyWithNewNode(itorID);
			return new Repeat0(expr, itorID);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			Expression.AddTransition(transitionMap, itorID);

			NFA.AddTransition(transitionMap, Start, Input.Empty, Node1);
			NFA.AddTransition(transitionMap, Start, Input.Empty, End);
			NFA.AddTransition(transitionMap, Node1, Input.Empty, Expression.Start);
			NFA.AddTransition(transitionMap, Expression.End, Input.Empty, Node2);
			NFA.AddTransition(transitionMap, Node2, Input.Empty, Node1);
			NFA.AddTransition(transitionMap, Node2, Input.Empty, End);
		}
	}
}
