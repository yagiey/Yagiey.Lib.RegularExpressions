using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using NFATransitionMap = IDictionary<TransitionParameters, IEnumerable<int>>;

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
			return
				TransitionMap
				.Aggregate(Enumerable.Empty<int>(), (sum, next) => sum = sum.Concat(next.Value).Append(next.Key.Node))
				.Concat(AcceptingNodeSet)
				.Distinct()
				.OrderBy(_ => _);
		}

		public IEnumerable<Input> GetAllInputs(bool includingEmpty)
		{
			var map =
				includingEmpty ?
				TransitionMap :
				TransitionMap.Where(t => !t.Key.Input.IsEmpty);

			return
				map
				.Select(t => t.Key.Input)
				.Distinct()
				.OrderBy(_ => _);
		}
	}
}
