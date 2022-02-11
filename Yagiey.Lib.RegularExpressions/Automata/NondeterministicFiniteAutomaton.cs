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
