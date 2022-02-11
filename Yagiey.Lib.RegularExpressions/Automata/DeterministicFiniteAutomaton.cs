using System;
using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using DFATransitionMap = IDictionary<int, IDictionary<Input, int>>;

	internal class DeterministicFiniteAutomaton : IDeterministicFiniteAutomaton<char>
	{
		private int _state;
		private bool _isError;

		public DeterministicFiniteAutomaton(int startNode, IEnumerable<int> acceptingNodeSet, DFATransitionMap transitionMap)
		{
			StartNode = startNode;
			AcceptingNodeSet = acceptingNodeSet;
			TransitionMap = transitionMap;
		}

		public int StartNode
		{
			get;
			private set;
		}

		public IEnumerable<int> AcceptingNodeSet
		{
			get;
			private set;
		}

		public DFATransitionMap TransitionMap
		{
			get;
			private set;
		}

		public static DFATransitionMap AddTransition(DFATransitionMap transitionMap, int node, Input input, int dest)
		{
			if (transitionMap.ContainsKey(node))
			{
				var dic = transitionMap[node];
				dic.Add(input, dest);
			}
			else
			{
				IDictionary<Input, int> dic = new Dictionary<Input, int>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}

		#region IDeterministicFiniteAutomaton

		public void Reset()
		{
			_state = 0;
			_isError = false;
		}

		public void MoveNext(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			if (!result.Item1)
			{
				_isError = true;
				return;
			}

			_state = result.Item2;
		}

		private static Tuple<bool, int> GetNext(char ch, int current, DFATransitionMap transitionMap, bool isError)
		{
			if (isError)
			{
				return Tuple.Create(false, 0);
			}

			bool result = transitionMap.TryGetValue(current, out IDictionary<Input, int>? dic);
			if (!result || dic == null)
			{
				return Tuple.Create(false, 0);
			}
			else
			{
				result = dic.TryGetValue(new Input(ch), out int dest);
				if (result)
				{
					return Tuple.Create(true, dest);
				}
				else
				{
					var dic2 = GetTransitionsExceptPositive(dic);
					foreach (var pair in dic2)
					{
						Input key = pair.Key;
						if (key.Match(ch))
						{
							return Tuple.Create(true, pair.Value);
						}
					}
					return Tuple.Create(false, 0);
				}
			}
		}

		private static IDictionary<Input, int> GetTransitionsExceptPositive(IDictionary<Input, int> map)
		{
			return new Dictionary<Input, int>(map.Where(_ => !_.Key.IsPositive && !_.Key.IsEmpty));
		}

		public bool IsInitialState()
		{
			return _state == 0;
		}

		public bool IsAcceptable()
		{
			return !IsError() && AcceptingNodeSet.Contains(_state);
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			foreach (char ch in source)
			{
				MoveNext(ch);
			}
			return IsAcceptable();
		}

		public bool IsError()
		{
			return _isError;
		}

		public bool IsNextError(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			return !result.Item1;
		}

		#endregion
	}
}
