using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using DFATransitionMap = IDictionary<TransitionParameters, int>;

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

		#region IDeterministicFiniteAutomaton

		public void Reset()
		{
			_state = 0;
			_isError = false;
		}

		public void MoveNext(char input)
		{
			if (IsError())
			{
				return;
			}

			TransitionParameters p = new(_state, new Input(input));
			bool result = TransitionMap.TryGetValue(p, out int dest);
			if (!result)
			{
				_isError = true;
			}
			else
			{
				_state = dest;
			}
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

		public bool IsNextError(char input)
		{
			if (IsError())
			{
				return true;
			}

			TransitionParameters p = new(_state, new Input(input));
			bool result = TransitionMap.TryGetValue(p, out _);
			if (!result)
			{
				return true;
			}

			return false;
		}

		#endregion
	}
}
