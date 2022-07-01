using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class Repeat1 : IExpression
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

		public int Node3
		{
			get;
			private set;
		}

		public int End
		{
			get;
			private set;
		}

		public Repeat1(IExpression expr, IEnumerator<int> itorID)
		{
			Expression = expr;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			Node1 = itorID.Current;
			itorID.MoveNext();
			Node2 = itorID.Current;
			itorID.MoveNext();
			Node3 = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public string ToRegularExpression()
		{
			if (Expression is Selection || Expression is Concatenation)
			{
				return Constants.LParen + Expression.ToRegularExpression() + Constants.RParen + Constants.Plus;
			}
			return Expression.ToRegularExpression() + Constants.Plus;
		}

		public IExpression CopyWithNewNode(IEnumerator<int> itorID)
		{
			IExpression expr = Expression.CopyWithNewNode(itorID);
			return new Repeat1(expr, itorID);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			IExpression expr = Expression.CopyWithNewNode(itorID);
			Expression.AddTransition(transitionMap, itorID);
			expr.AddTransition(transitionMap, itorID);

			NFA.AddTransition(transitionMap, Start, Input.Empty, Expression.Start);
			NFA.AddTransition(transitionMap, Expression.End, Input.Empty, Node1);
			NFA.AddTransition(transitionMap, Node1, Input.Empty, End);
			NFA.AddTransition(transitionMap, Node1, Input.Empty, Node2);
			NFA.AddTransition(transitionMap, Node2, Input.Empty, expr.Start);
			NFA.AddTransition(transitionMap, expr.End, Input.Empty, Node3);
			NFA.AddTransition(transitionMap, Node3, Input.Empty, Node2);
			NFA.AddTransition(transitionMap, Node3, Input.Empty, End);
		}
	}
}
