using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using NFATransitionMap = IDictionary<int, IDictionary<Input, IEnumerable<int>>>;

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
			IEnumerable<int> result = Enumerable.Empty<int>();
			foreach (var pair1 in TransitionMap)
			{
				int node = pair1.Key;
				result = result.Append(node);
				IDictionary<Input, IEnumerable<int>> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					IEnumerable<int> destination = pair2.Value;
					result = result.Concat(destination);
				}
			}
			return result.Distinct().OrderBy(_ => _);
		}

		public IEnumerable<Input> GetAllInputs(bool includingEmpty)
		{
			IEnumerable<Input> result = Enumerable.Empty<Input>();
			foreach (var pair1 in TransitionMap)
			{
				IDictionary<Input, IEnumerable<int>> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					Input input = pair2.Key;
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
				var strMap = string.Join(",", pair.Value.OrderBy(_ => _.Key).Select(pair2 => string.Format("ch({0})->[{1}]", (int)pair2.Key.Character, string.Join(",", pair2.Value.OrderBy(_ => _)))));
				lines.Add(string.Format("{0}:{1}", pair.Key, strMap));
			}
			return string.Join("\r\n", lines);
		}

		public static NFATransitionMap AddTransition(NFATransitionMap transitionMap, int node, Input input, IEnumerable<int> dest)
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
				IDictionary<Input, IEnumerable<int>> dic = new Dictionary<Input, IEnumerable<int>>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}
	}
}
