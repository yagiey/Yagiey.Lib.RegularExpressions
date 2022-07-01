using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class Character : IExpression
	{
		public IInput Input
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

		public Character(IInput input, IEnumerator<int> itorID)
		{
			Input = input;
			itorID.MoveNext();
			Start = itorID.Current;
			itorID.MoveNext();
			End = itorID.Current;
		}

		public Character(IInput input, int start, int end)
		{
			Input = input;
			Start = start;
			End = end;
		}

		public string ToRegularExpression()
		{
			return Input.ToRegularExpression();
		}

		public IExpression CopyWithNewNode(IEnumerator<int> itorID)
		{
			itorID.MoveNext();
			int start = itorID.Current;
			itorID.MoveNext();
			int end = itorID.Current;
			return new Character(Input, start, end);
		}

		public void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			bool found = transitionMap.TryGetValue(Start, out IDictionary<IInput, IEnumerable<int>>? dic);
			if (!found || dic == null)
			{
				transitionMap.Add(Start, new Dictionary<IInput, IEnumerable<int>> { { Input, new int[] { End } } });
			}
			else
			{
				found = dic.TryGetValue(Input, out IEnumerable<int>? end);
				if (!found || end == null)
				{
					dic.Add(Input, new int[] { End });
				}
				else
				{
					dic[Input] = end.Append(End);
				}
			}
		}
	}
}
