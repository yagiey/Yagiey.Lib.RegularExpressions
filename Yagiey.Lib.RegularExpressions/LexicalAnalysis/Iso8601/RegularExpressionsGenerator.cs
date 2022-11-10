using System;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.Iso8601
{
	using DFA = IDeterministicFiniteAutomaton<char>;

	public class RegularExpressionsGenerator
	{
		public static DFA Date()
		{
			Tuple<string, string> patterns = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			return new NegativeLookahead(patterns.Item1, patterns.Item2, false);
		}

		public static DFA Date(StringAffix affix)
		{
			Tuple<string, string> patterns = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			return new NegativeLookahead(affix, patterns.Item1, patterns.Item2, false);
		}

		public static DFA Time()
		{
			const int DigitsFsec = 9;
			string pattern = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new RegularExpression(pattern, false);
		}

		public static DFA Time(StringAffix affix)
		{
			const int DigitsFsec = 9;
			string pattern = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			string prefix = Constants.Sanitize(affix.Prefix);
			string suffix = Constants.Sanitize(affix.Suffix);
			return new RegularExpression($"{prefix}({pattern}){suffix}", false);
		}

		public static DFA DateTime()
		{
			const int DigitsFsec = 9;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new NegativeLookahead(patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}", false);
		}

		public static DFA DateTime(StringAffix affix)
		{
			const int DigitsFsec = 9;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new NegativeLookahead(affix, patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}", false);
		}

		public static DFA DateTimeOffset()
		{
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn][{+|-}hh:mm]
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn]Z (UTC)
			const int DigitsFsec = 9;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			string patternOffset = LexicalAnalysis.RegularExpressionsGenerator.GetOffsetPattern();
			return new NegativeLookahead(patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}{patternOffset}", false);
		}

		public static DFA DateTimeOffset(StringAffix affix)
		{
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn][{+|-}hh:mm]
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn]Z (UTC)
			const int DigitsFsec = 9;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			string patternOffset = LexicalAnalysis.RegularExpressionsGenerator.GetOffsetPattern();
			return new NegativeLookahead(affix, patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}{patternOffset}", false);
		}
	}
}
