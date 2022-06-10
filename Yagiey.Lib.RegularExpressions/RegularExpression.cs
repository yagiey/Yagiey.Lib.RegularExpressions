using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions
{
	using DFA = DeterministicFiniteAutomaton;
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	public class RegularExpression : IDeterministicFiniteAutomaton<char>
	{
		private readonly DFA _dfa;

		public string Source
		{
			get;
			private set;
		}

		public RegularExpression(string source, bool ignoreCase)
		{
			Parser parser = new(source);
			Source = source;
			NFA nfa = ToNFA(parser.EpsilonNFA);
			DFA dfa = ToDFA(nfa, ignoreCase);
			_dfa = MinimizeDFA(dfa, ignoreCase);
		}

		public override string ToString()
		{
			return _dfa.ToString();
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

			NFATransitionMap newTransitionMap = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
			foreach (var node in allNodes)
			{
				foreach (IInput input in allInputs)
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
			transitions.TryGetValue(node, out IDictionary<IInput, IEnumerable<int>>? dic);
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

		private static IEnumerable<int> TransitionFunction(IEnumerable<int> nodeSet, IInput input, NFATransitionMap transitionMap)
		{
			IEnumerable<int> ret = Enumerable.Empty<int>();
			foreach (int node in nodeSet)
			{
				IEnumerable<int>? e;
				transitionMap.TryGetValue(node, out IDictionary<IInput, IEnumerable<int>>? dic);
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

		private static IEnumerable<int> TransitionFunction(int node, IInput input, NFATransitionMap transitionMap, IDictionary<int, IEnumerable<int>> eClosure)
		{
			var c = EpsilonClosure(new int[] { node }, eClosure);
			var e = TransitionFunction(c, input, transitionMap);
			return EpsilonClosure(e, eClosure);
		}

		#endregion

		#region conversion NFA to DFA

		private static DFA ToDFA(NFA nfa, bool ignoreCase)
		{
			IEnumerable<IInput> allInputs = nfa.GetAllInputs(false);
			IEnumerable<int> nodeSet = new int[] { nfa.StartNode };
			var result = SubsetConstruction(nodeSet, allInputs, nfa.TransitionMap);

			var dfaTransitionMap = result.Item1;
			var nodeMap = result.Item2;
			int startNode = nodeMap[nodeSet];
			var acceptingNodeSet = nodeMap.Where(_ => _.Key.Intersect(nfa.AcceptingNodeSet).Any()).Select(_ => _.Value).Distinct();

			return new DFA(startNode, acceptingNodeSet, dfaTransitionMap, ignoreCase);
		}

		private static Tuple<DFATransitionMap, IDictionary<IEnumerable<int>, int>> SubsetConstruction(IEnumerable<int> start, IEnumerable<IInput> allInputs, NFATransitionMap transitionMap)
		{
			IEnumerator<int> itorID = new SequenceNumberEnumerator(0);
			DFATransitionMap newMap = new Dictionary<int, IDictionary<IInput, int>>();
			IDictionary<IEnumerable<int>, int> done = new Dictionary<IEnumerable<int>, int>(new IntSetEqualityComparer());

			SubsetConstruction(start, allInputs, transitionMap, itorID, newMap, done);

			return Tuple.Create(newMap, done);
		}

		private static void SubsetConstruction(IEnumerable<int> nodeSet, IEnumerable<IInput> allInputs, NFATransitionMap transitionMap, IEnumerator<int> itorID, DFATransitionMap newMap, IDictionary<IEnumerable<int>, int> done)
		{
			if (done.ContainsKey(nodeSet))
			{
				return;
			}

			itorID.MoveNext();
			int id = itorID.Current;
			done.Add(nodeSet, id);

			foreach (IInput input in allInputs)
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

		#region DFA minimization

		private static DFA MinimizeDFA(DFA dfa, bool ignoreCase)
		{
			IDictionary<int, HashSet<int>> group2Nodes = Create1stGenerationGroups(dfa);

			IEqualityComparer<IEnumerable<int>> eqComp = new EnumerableEqualityComparer<int>();
			IDictionary<int, int> node2Group;
			while (true)
			{
				var newGroups = MakeGroup(group2Nodes, dfa.TransitionMap);
				IDictionary<int, HashSet<int>> group2NodesNew = newGroups.Item2;

				bool isEq =
					group2Nodes.Values.All(o => group2NodesNew.Values.Any(n => eqComp.Equals(o, n)))
					 && group2NodesNew.Values.All(n => group2Nodes.Values.Any(o => eqComp.Equals(n, o)));

				group2Nodes = group2NodesNew;
				node2Group = newGroups.Item1;
				if (isEq)
				{
					break;
				}
			}

			// make new DFA
			bool isAcc(int node) => dfa.AcceptingNodeSet.Any(_ => _ == node);
			int startNode = -1;
			IEnumerable<int> acceptingNodeSet = Enumerable.Empty<int>();
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>();
			foreach (var pair in group2Nodes)
			{
				int groupID = pair.Key;
				IEnumerable<int> members = pair.Value;

				if (members.Any(_ => isAcc(_)))
				{
					acceptingNodeSet = acceptingNodeSet.Append(groupID);
				}

				if (members.Any(_ => _ == dfa.StartNode))
				{
					startNode = groupID;
				}

				int nodeIDOrg = members.First();

				dfa.TransitionMap.TryGetValue(nodeIDOrg, out IDictionary<IInput, int>? trans);
				if (trans == null)
				{
					continue;
				}

				IDictionary<IInput, int> transNew =
					new Dictionary<IInput, int>(
						trans.Select(_ => new KeyValuePair<IInput, int>(_.Key, node2Group[_.Value]))
					);
				transitionMap.Add(groupID, transNew);
			}

			return new DFA(startNode, acceptingNodeSet, transitionMap, ignoreCase);
		}

		private static IDictionary<int, HashSet<int>> Create1stGenerationGroups(DFA dfa)
		{
			bool isAcceptable(int node) => dfa.AcceptingNodeSet.Any(_ => _ == node);

			IDictionary<int, HashSet<int>> result = new Dictionary<int, HashSet<int>>();
			foreach (int node in dfa.GetAllNodes())
			{
				int group = isAcceptable(node) ? 1 : 0;
				if (result.ContainsKey(group))
				{
					result[group].Add(node);
				}
				else
				{
					result.Add(group, new HashSet<int> { node });
				}
			}
			return result;
		}

		private static Tuple<IDictionary<int, int>, IDictionary<int, HashSet<int>>> MakeGroup(IDictionary<int, HashSet<int>> group2Nodes, DFATransitionMap transitionMap)
		{
			SequenceNumberEnumerator itorID = new(0);
			IDictionary<int, HashSet<int>> group2NodesNew = new Dictionary<int, HashSet<int>>();
			IDictionary<int, int> node2GroupNew = new Dictionary<int, int>();
			IDictionary<string, int> map = new Dictionary<string, int>();

			foreach (int groupID in group2Nodes.Keys)
			{
				foreach (int nodeID in group2Nodes[groupID])
				{
					transitionMap.TryGetValue(nodeID, out IDictionary<IInput, int>? trans);

					string id;
					if (trans == null)
					{
						id = "EMPTY";
					}
					else
					{
						id = string.Join(
							"|",
							trans.OrderBy(_ => _.Key).Select(_ => string.Format("{0}:{1}", _.Key is Input ? string.Format("(ch {0})", (int)(_.Key as Input)!.Character) : _.Key.ToString(), _.Value))
						);
					}

					if (map.TryGetValue(id, out int newGroupID))
					{
						group2NodesNew[newGroupID].Add(nodeID);
					}
					else
					{
						itorID.MoveNext();
						newGroupID = itorID.Current;
						group2NodesNew.Add(newGroupID, new HashSet<int> { nodeID });
						map.Add(id, newGroupID);
					}
					node2GroupNew.Add(nodeID, newGroupID);
				}
				map.Clear();
			}
			return Tuple.Create(node2GroupNew, group2NodesNew);
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
