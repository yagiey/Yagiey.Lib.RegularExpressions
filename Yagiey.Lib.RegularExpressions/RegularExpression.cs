using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions
{
	using DFA = DeterministicFiniteAutomaton;
	using DFATransitionMap = IDictionary<int, IDictionary<Input, int>>;
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<Input, IEnumerable<int>>>;

	public class RegularExpression : IDeterministicFiniteAutomaton<char>
	{
		private readonly DFA _dfa;

		public RegularExpression(string source)
		{
			Parser parser = new(source);
			NFA nfa = ToNFA(parser.EpsilonNFA);
			_dfa = ToDFA(nfa);
		}

		#region conversion εNFA to NFA

		private static NFA ToNFA(NFA epsilonNFA)
		{
			int startNode = epsilonNFA.StartNode;
			IEnumerable<int> acceptingNodeSet = epsilonNFA.AcceptingNodeSet;
			NFATransitionMap transitionMap = epsilonNFA.TransitionMap;

			var allNodes = epsilonNFA.GetAllNodes();
			var allInputs = epsilonNFA.GetAllInputs(false);

			IDictionary<int, IEnumerable<int>> eClosures = new Dictionary<int, IEnumerable<int>>();
			foreach (var n in allNodes)
			{
				var ec = EpsilonClosure(n, transitionMap, eClosures);
				var pair = new KeyValuePair<int, IEnumerable<int>>(n, ec);
				if (!eClosures.ContainsKey(n))
				{
					eClosures.Add(pair);
				}
			}

			IEnumerable<int> newAcceptingNodeSet;
			if (eClosures[startNode].Intersect(acceptingNodeSet).Any())
			{
				newAcceptingNodeSet = acceptingNodeSet.Union(new int[] { startNode }).Distinct();
			}
			else
			{
				newAcceptingNodeSet = acceptingNodeSet;
			}

			NFATransitionMap newTransitionMap = new Dictionary<int, IDictionary<Input, IEnumerable<int>>>();
			foreach (var node in allNodes)
			{
				foreach (var input in allInputs)
				{
					var destination = TransitionFunction(node, input, transitionMap, eClosures).Distinct().ToArray();
					if (destination.Length == 0)
					{
						continue;
					}
					NFA.AddTransition(newTransitionMap, node, input, destination);
				}
			}

			return new NFA(startNode, newAcceptingNodeSet, newTransitionMap);
		}

		private static IEnumerable<int> EpsilonClosure(int node, NFATransitionMap transitions, IDictionary<int, IEnumerable<int>> eClosure)
		{
			if (eClosure.ContainsKey(node))
			{
				return eClosure[node];
			}

			return EpsilonClosure(node, transitions, eClosure, Enumerable.Empty<int>());
		}

		private static IEnumerable<int> EpsilonClosure(int node, NFATransitionMap transitions, IDictionary<int, IEnumerable<int>> eClosure, IEnumerable<int> done)
		{
			if (eClosure.ContainsKey(node))
			{
				return eClosure[node];
			}

			IEnumerable<int>? e;
			transitions.TryGetValue(node, out IDictionary<Input, IEnumerable<int>>? dic);
			if (dic == null)
			{
				return new int[] { node };
			}
			else
			{
				dic.TryGetValue(Input.Empty, out e);
				if (e == null)
				{
					return new int[] { node };
				}
			}

			var destinations = e.Append(node);
			done = done.Append(node);

			var l = destinations.ToList();
			foreach (int n in l)
			{
				if (done.Contains(n))
				{
					continue;
				}

				if (eClosure.ContainsKey(n))
				{
					destinations = destinations.Concat(eClosure[n]);
				}
				else
				{
					var d = EpsilonClosure(n, transitions, eClosure, done);
					eClosure.Add(n, d);
					destinations = destinations.Concat(d);
				}
			}

			return destinations.Distinct();
		}

		private static IEnumerable<int> TransitionFunction(IEnumerable<int> nodeSet, Input input, NFATransitionMap transitionMap)
		{
			IEnumerable<int> ret = Enumerable.Empty<int>();
			foreach (int node in nodeSet)
			{
				IEnumerable<int>? e;
				transitionMap.TryGetValue(node, out IDictionary<Input, IEnumerable<int>>? dic);
				if (dic == null)
				{
					continue;
				}
				else
				{
					dic.TryGetValue(input, out e);
					if (e == null || !e.Any())
					{
						continue;
					}
				}
				ret = ret.Concat(e);
			}
			return ret;
		}

		private static IEnumerable<int> EpsilonClosure(IEnumerable<int> nodeSet, IDictionary<int, IEnumerable<int>> eClosure)
		{
			var result = Enumerable.Empty<int>();
			foreach (int node in nodeSet)
			{
				result = result.Concat(eClosure[node]);
			}
			return result.Distinct();
		}

		private static IEnumerable<int> TransitionFunction(int node, Input input, NFATransitionMap transitionMap, IDictionary<int, IEnumerable<int>> eClosure)
		{
			var c = EpsilonClosure(new int[] { node }, eClosure);
			var e = TransitionFunction(c, input, transitionMap);
			return EpsilonClosure(e, eClosure);
		}

		#endregion

		#region conversion NFA to DFA

		private static DFA ToDFA(NFA nfa)
		{
			IEnumerable<Input> allInputs = nfa.GetAllInputs(false);
			IEnumerator<int> itorID = new SequenceNumberEnumerator(0);

			IEnumerable<int> nodeSet = new int[] { nfa.StartNode };
			var result = SubsetConstruction(nodeSet, allInputs, nfa.TransitionMap, itorID);

			var dfaTransitionMap = result.Item1;
			var nodeMap = result.Item2;
			int startNode = nodeMap[nodeSet];
			var acceptingNodeSet = nodeMap.Where(_ => _.Key.Intersect(nfa.AcceptingNodeSet).Any()).Select(_ => _.Value).Distinct();

			return new DFA(startNode, acceptingNodeSet, dfaTransitionMap);
		}

		private static Tuple<DFATransitionMap, IDictionary<IEnumerable<int>, int>> SubsetConstruction(IEnumerable<int> start, IEnumerable<Input> allInputs, NFATransitionMap transitionMap, IEnumerator<int> itorID)
		{
			DFATransitionMap newMap = new Dictionary<int, IDictionary< Input, int>> ();
			IDictionary<IEnumerable<int>, int> done = new Dictionary<IEnumerable<int>, int>(new IntSetEqualityComparer());

			SubsetConstruction(start, allInputs, transitionMap, itorID, newMap, done);

			return Tuple.Create(newMap, done);
		}

		private static void SubsetConstruction(IEnumerable<int> nodeSet, IEnumerable<Input> allInputs, NFATransitionMap transitionMap, IEnumerator<int> itorID, DFATransitionMap newMap, IDictionary<IEnumerable<int>, int> done)
		{
			if (done.ContainsKey(nodeSet))
			{
				return;
			}

			itorID.MoveNext();
			int id = itorID.Current;
			done.Add(nodeSet, id);

			foreach (Input input in allInputs)
			{
				IEnumerable<int> destinations = TransitionFunction(nodeSet, input, transitionMap);
				if (destinations.Any())
				{
					destinations = destinations.Distinct();
					SubsetConstruction(destinations, allInputs, transitionMap, itorID, newMap, done);

					int next = done[destinations];
					DFA.AddTransition(newMap, id, input, next);
				}
			}
		}

		#endregion

		#region IDeterministicFiniteAutomaton

		public void Reset()
		{
			_dfa.Reset();
		}

		public void MoveNext(char input)
		{
			_dfa.MoveNext(input);
		}

		public bool IsInitialState()
		{
			return _dfa.IsInitialState();
		}

		public bool IsAcceptable()
		{
			return _dfa.IsAcceptable();
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
			return _dfa.IsError();
		}

		public bool IsNextError(char input)
		{
			return _dfa.IsNextError(input);
		}

		#endregion
	}
}
