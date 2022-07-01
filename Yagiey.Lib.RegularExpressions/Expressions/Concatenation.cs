using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class Concatenation : IExpression
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

		public Concatenation(IEnumerable<IExpression> expressions, IEnumerator<int> itorID)
		{
			Expressions = expressions;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public string ToRegularExpression()
		{
			var result = string.Join("", Expressions.Select(e => e is Selection ? Constants.LParen + e.ToRegularExpression() + Constants.RParen : e.ToRegularExpression()));
			return result;
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
			return new Concatenation(exprs, itorID);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			IExpression prev = Expressions.First();
			prev.AddTransition(transitionMap, itorID);
			NFA.AddTransition(transitionMap, Start, Input.Empty, prev.Start);
			foreach (IExpression expr in Expressions.Skip(1))
			{
				expr.AddTransition(transitionMap, itorID);
				NFA.AddTransition(transitionMap, prev.End, Input.Empty, expr.Start);
				prev = expr;
			}
			NFA.AddTransition(transitionMap, prev.End, Input.Empty, End);
		}
	}
}
