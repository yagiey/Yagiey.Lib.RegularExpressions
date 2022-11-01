using System;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.W3cXml
{
	using DFA = IDeterministicFiniteAutomaton<char>;

	public class RegularExpressionsGenerator
	{
		public static DFA Date()
		{
			return Date(StringAffix.Empty);
		}

		public static DFA Date(StringAffix affix)
		{
			Tuple<string, string> patternsDate = Iso8601.RegularExpressionsGenerator.GetDatePattern();
			string patternOffset = Iso8601.RegularExpressionsGenerator.GetOffsetPattern();
			return new NegativeLookahead(affix, patternsDate.Item1, @$"({patternsDate.Item2})({patternOffset})", false);
		}
	}
}
