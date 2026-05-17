using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal class NondeterministicFiniteAutomaton
	{
		public NondeterministicFiniteAutomaton(int startNode, IEnumerable<int> acceptingNodeSet, NFATransitionMap transitionMap)
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

		public NFATransitionMap TransitionMap
		{
			get;
			private set;
		}

		public IEnumerable<int> GetAllNodes()
		{
			HashSet<int> result = new();
			foreach (var pair1 in TransitionMap)
			{
				int node = pair1.Key;
				result.Add(node);
				IDictionary<IInput, IEnumerable<int>> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					IEnumerable<int> destination = pair2.Value;
					foreach (int dest in destination)
					{
						result.Add(dest);
					}
				}
			}
			return result.OrderBy(it => it);
		}

		public IEnumerable<IInput> GetAllInputs(bool includingEmpty)
		{
			IEnumerable<IInput> result = Enumerable.Empty<IInput>();
			foreach (var pair1 in TransitionMap)
			{
				IDictionary<IInput, IEnumerable<int>> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					IInput input = pair2.Key;
					if (!includingEmpty && input.IsEmpty)
					{
						continue;
					}
					result = result.Append(input);
				}
			}
			return result.Distinct().OrderBy(_ => _);
		}

		public override string ToString()
		{
			List<string> lines = new();
			lines.Add(string.Format("start: {0}", StartNode));
			lines.Add(string.Format("accepting: [{0}]", string.Join(",", AcceptingNodeSet)));
			foreach (var pair in TransitionMap.OrderBy(_ => _.Key))
			{
				var strMap = string.Join(",", pair.Value.OrderBy(_ => _.Key).Select(pair2 => string.Format("{0}->[{1}]", pair2.Key is Input ? string.Format("(ch {0})", (int)(pair2.Key as Input)!.Character) : pair2.Key.ToString(), string.Join(",", pair2.Value.OrderBy(_ => _)))));
				lines.Add(string.Format("{0}:{1}", pair.Key, strMap));
			}
			return string.Join("\r\n", lines);
		}

		public static void AddTransition(NFATransitionMap transitionMap, int start, IInput input, int end)
		{
			bool found = transitionMap.TryGetValue(start, out IDictionary<IInput, IEnumerable<int>>? dic);
			if (!found || dic == null)
			{
				transitionMap.Add(start, new Dictionary<IInput, IEnumerable<int>> { { input, new int[] { end } } });
			}
			else
			{
				found = dic.TryGetValue(input, out IEnumerable<int>? e);
				if (!found || e == null)
				{
					dic.Add(input, new int[] { end });
				}
				else
				{
					if (!e.Contains(end))
					{
						dic[input] = e.Append(end);
					}
				}
			}
		}

		public static void AddNormalizedTransition(NFATransitionMap transitionMap, int state, IInput condition, int nextState)
		{
			if (condition is Input input)
			{
				AddTransition(transitionMap, state, input, nextState);
			}
			else if (condition is InputWithPredicate cond)
			{
				if (cond.Predicate is Not<char>)
				{
					// Not<T> 条件式を InputWithPredicate を利用して遷移規則に保持
					AddTransition(transitionMap, state, cond, nextState);
				}
				else if (cond.Predicate is And<char>)
				{
					// And 条件式を InputWithPredicate を利用して遷移規則に保持
					AddTransition(transitionMap, state, cond, nextState);
				}
				else if (cond.Predicate is GELE<char> range)
				{
					// GELE 条件をリテラル集合に展開
					for (char c = range.LowerLimit; c <= range.UpperLimit; c++)
					{
						AddTransition(transitionMap, state, new Input(c), nextState);
					}
				}
				else if (cond.Predicate is Or<char> orCondition)
				{
					// Or 条件を個別条件に分解
					foreach (var subCondition in orCondition.Predicates)
					{
						AddNormalizedTransition(transitionMap, state, new InputWithPredicate(subCondition), nextState);
					}
				}
				else if (cond.Predicate is Equal<char> equalCondition)
				{
					var c = equalCondition.Value;
					AddTransition(transitionMap, state, new Input(c), nextState);
				}
				else
				{
					throw new ArgumentException("未対応の条件式が指定されました");
				}
			}
			else
			{
				throw new ArgumentException("未対応の条件式が指定されました");
			}
		}

		public static NFATransitionMap AddTransition(NFATransitionMap transitionMap, int node, IInput input, IEnumerable<int> dest)
		{
			if (transitionMap.ContainsKey(node))
			{
				var dic = transitionMap[node];
				if (dic.ContainsKey(input))
				{
					var value = dic[input];
					dic[input] = value.Concat(dest);
				}
				else
				{
					dic.Add(input, dest);
				}
			}
			else
			{
				IDictionary<IInput, IEnumerable<int>> dic = new Dictionary<IInput, IEnumerable<int>>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}
	}
}
