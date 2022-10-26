using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions
{
	public class NegativeLookahead : IDeterministicFiniteAutomaton<char>
	{
		private readonly RegularExpression _head;
		private readonly RegularExpression _main;

		public NegativeLookahead(string patternNegativeHead, string patternMain, bool ignoreCase)
		{
			const string repeat = @"(.*)";

			_head = new($"({patternNegativeHead}){repeat}", ignoreCase);
			_main = new(patternMain, ignoreCase);
			NegativeHead = _head.Source!;
			Main = _main.Source!;
		}

		public NegativeLookahead(StringAffix affix, string patternNegativeHead, string patternMain, bool ignoreCase)
		{
			const string repeat = @"(.*)";
			string prefix = Constants.Sanitize(affix.Prefix);
			string suffix = Constants.Sanitize(affix.Suffix);

			_head = new($"{prefix}({patternNegativeHead}){repeat}", ignoreCase);
			_main = new($"{prefix}({patternMain}){suffix}", ignoreCase);
			NegativeHead = _head.Source!;
			Main = _main.Source!;
		}

		public string NegativeHead
		{
			get;
			private set;
		}

		public string Main
		{
			get;
			private set;
		}

		#region IDeterministicFiniteAutomaton<char>

		public bool IsAcceptable()
		{
			return !_head.IsAcceptable() && _main.IsAcceptable();
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			return !_head.IsAcceptable(source) && _main.IsAcceptable(source);
		}

		public bool IsError()
		{
			return _head.IsAcceptable() || _main.IsError();
		}

		public bool IsInitialState()
		{
			return _head.IsInitialState() && _main.IsInitialState();
		}

		public bool IsNextError(char input)
		{
			return _head.IsNextAcceptable(input) || _main.IsNextError(input);
		}

		public bool IsNextAcceptable(char input)
		{
			return !_head.IsNextAcceptable(input) && _main.IsNextAcceptable(input);
		}

		public void MoveNext(char input)
		{
			_head.MoveNext(input);
			_main.MoveNext(input);
		}

		public void Reset()
		{
			_head.Reset();
			_main.Reset();
		}

		#endregion
	}
}
