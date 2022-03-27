using System;
using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

	internal class DeterministicFiniteAutomaton : IDeterministicFiniteAutomaton<char>
	{
		private int _state;
		private bool _isError;

		public DeterministicFiniteAutomaton(int startNode, IEnumerable<int> acceptingNodeSet, DFATransitionMap transitionMap, bool ignoreCase)
		{
			StartNode = startNode;
			AcceptingNodeSet = acceptingNodeSet;
			TransitionMap = transitionMap;
			IgnoreCase = ignoreCase;
		}

		public bool IgnoreCase
		{
			get;
			private set;
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

		public IEnumerable<int> GetAllNodes()
		{
			IEnumerable<int> result = Enumerable.Empty<int>();
			foreach (var pair1 in TransitionMap)
			{
				int node = pair1.Key;
				result = result.Append(node);
				IDictionary<IInput, int> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					int destination = pair2.Value;
					result = result.Append(destination);
				}
			}
			return result.Distinct().OrderBy(_ => _);
		}

		public static DFATransitionMap AddTransition(DFATransitionMap transitionMap, int node, IInput input, int dest)
		{
			if (transitionMap.ContainsKey(node))
			{
				var dic = transitionMap[node];
				dic.Add(input, dest);
			}
			else
			{
				IDictionary<IInput, int> dic = new Dictionary<IInput, int>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}

		public override string ToString()
		{
			List<string> lines = new();
			lines.Add(string.Format("start: {0}", StartNode));
			lines.Add(string.Format("accepting: [{0}]", string.Join(",", AcceptingNodeSet)));
			foreach (var pair in TransitionMap.OrderBy(_ => _.Key))
			{
				var strMap = string.Join(",", pair.Value.OrderBy(_ => _.Key).Select(pair2 => string.Format("{0}->{1}", pair2.Key is Input ? string.Format("ch({0})", (int)(pair2.Key as Input)!.Character) : pair2.Key.ToString(), pair2.Value)));
				lines.Add(string.Format("{0}:{1}", pair.Key, strMap));
			}
			return string.Join("\r\n", lines);
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
			if (result.Item1)
			{
				_state = result.Item2;
				return;
			}

			if (IgnoreCase)
			{
				char? corr = ToCorrespondingCase(ch);
				if (corr.HasValue)
				{
					var result2 = GetNext(corr.Value, _state, TransitionMap, IsError());
					if (result2.Item1)
					{
						_state = result2.Item2;
						return;
					}
				}
			}

			_isError = true;
		}

		private static Tuple<bool, int> GetNext(char ch, int current, DFATransitionMap transitionMap, bool isError)
		{
			if (isError)
			{
				return Tuple.Create(false, 0);
			}

			bool result = transitionMap.TryGetValue(current, out IDictionary<IInput, int>? dic);
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
					var dic2 = GetTransitionsWithPredicate(dic);
					foreach (var pair in dic2)
					{
						IInput key = pair.Key;
						if (key.Match(ch))
						{
							return Tuple.Create(true, pair.Value);
						}
					}
					return Tuple.Create(false, 0);
				}
			}
		}

		private static IDictionary<IInput, int> GetTransitionsWithPredicate(IDictionary<IInput, int> map)
		{
			return new Dictionary<IInput, int>(map.Where(_ => _.Key.IsPredicate));
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

		private static char? ToCorrespondingCase(char ch)
		{
			if (IsUpperCase(ch))
			{
				return char.ToLower(ch);
			}
			else if (IsLowerCase(ch))
			{
				return char.ToUpper(ch);
			}
			else
			{
				return null;
			}
		}

		private static bool IsUpperCase(char ch)
		{
			return 'A' <= ch && ch <= 'Z';
		}

		private static bool IsLowerCase(char ch)
		{
			return 'a' <= ch && ch <= 'z';
		}
	}
}
