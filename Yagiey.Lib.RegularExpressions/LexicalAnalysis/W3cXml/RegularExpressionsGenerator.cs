using System;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.W3cXml
{
	using DFA = IDeterministicFiniteAutomaton<char>;

	public class RegularExpressionsGenerator
	{
		public static DFA Date()
		{
			Tuple<string, string> patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternOffset = LexicalAnalysis.RegularExpressionsGenerator.GetOffsetPattern();
			return new NegativeLookahead(patternsDate.Item1, @$"({patternsDate.Item2})({patternOffset})", false);
		}

		public static DFA Date(StringAffix affix)
		{
			Tuple<string, string> patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternOffset = LexicalAnalysis.RegularExpressionsGenerator.GetOffsetPattern();
			return new NegativeLookahead(affix, patternsDate.Item1, @$"({patternsDate.Item2})({patternOffset})", false);
		}
	}
}
