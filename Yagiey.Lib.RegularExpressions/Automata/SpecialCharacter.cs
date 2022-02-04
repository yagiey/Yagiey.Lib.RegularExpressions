using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal class SpecialCharacter : IDeterministicFiniteAutomaton<char>
	{
		private enum States
		{
			Error,
			Initial,
			State0,
		}

		private States _state;

		public SpecialCharacter()
		{
			Reset();
		}

		public void Reset()
		{
			_state = States.Initial;
		}

		public void MoveNext(char ch)
		{
			States next = GetNext(ch);
			_state = next;
		}

		public bool IsInitialState()
		{
			return _state == States.Initial;
		}

		public bool IsAcceptable()
		{
			return IsAcceptable(_state);
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			Reset();
			foreach (char ch in source)
			{
				_state = GetNext(ch);
				if (_state == States.Error)
				{
					break;
				}
			}
			return IsAcceptable();
		}

		public bool IsError()
		{
			return IsError(_state);
		}

		public bool IsNextError(char ch)
		{
			States next = GetNext(ch);
			return IsError(next);
		}

		private States GetNext(char ch)
		{
			States next;
			switch (_state)
			{
				case States.Initial:
					if (Constants.SpecialCharacters.Contains(ch))
					{
						next = States.State0;
					}
					else
					{
						next = States.Error;
					}
					break;

				case States.State0:
				case States.Error:
				default:
					next = States.Error;
					break;
			}

			return next;
		}

		private static bool IsError(States s)
		{
			return s == States.Error;
		}

		private static bool IsAcceptable(States s)
		{
			return s == States.State0;
		}
	}
}
