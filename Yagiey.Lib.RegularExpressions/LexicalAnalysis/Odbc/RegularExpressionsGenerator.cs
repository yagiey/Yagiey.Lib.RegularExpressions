using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.Odbc
{
	using DFA = IDeterministicFiniteAutomaton<char>;

	/// <summary></summary>
	/// <see cref="https://learn.microsoft.com/en-us/sql/odbc/reference/appendixes/date-time-and-timestamp-escape-sequences?view=sql-server-ver16"/>
	public class RegularExpressionsGenerator
	{
		public static DFA Date()
		{
			// {d '1995-01-15'}
			var patterns = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string pat1 = @$"\{{\s*(d|D)\s*'({patterns.Item1})";
			string pat2 = @$"\{{\s*(d|D)\s*'({patterns.Item2})'\s*\}}";
			return new NegativeLookahead($"{pat1}", $"{pat2}", false);
		}

		public static DFA Date(StringAffix affix)
		{
			// {d '1995-01-15'}
			var patterns = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string pat1 = @$"\{{\s*(d|D)\s*'({patterns.Item1})";
			string pat2 = @$"\{{\s*(d|D)\s*'({patterns.Item2})'\s*\}}";
			return new NegativeLookahead(affix, pat1, pat2, false);
		}

		public static DFA Time()
		{
			// {t 'hh:mm:ss[.fractional seconds]'}
			const int DigitsFsec = 7;
			string pattern = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new RegularExpression(@$"\{{\s*(t|T)\s*'({pattern})'\s*\}}", false);
		}

		public static DFA Time(StringAffix affix)
		{
			// {t 'hh:mm:ss[.fractional seconds]'}
			string prefix = Constants.Sanitize(affix.Prefix);
			string suffix = Constants.Sanitize(affix.Suffix);
			const int DigitsFsec = 7;
			string pattern = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new RegularExpression(@$"{prefix}\{{\s*(t|T)\s*'({pattern})'\s*\}}{suffix}", false);
		}

		public static DFA DateTime()
		{
			// { ts 'yyyy-mm-dd hh:mm:ss[.fractional seconds]' }
			const int DigitsFsec = 7;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new NegativeLookahead(@$"\{{\s*((t|T)(s|S))\s*'{patternsDate.Item1}", @$"\{{\s*((t|T)(s|S))\s*'{patternsDate.Item2} {patternTime}'\s*\}}", false);
		}

		public static DFA DateTime(StringAffix affix)
		{
			// { ts 'yyyy-mm-dd hh:mm:ss[.fractional seconds]' }
			const int DigitsFsec = 7;
			var patternsDate = LexicalAnalysis.RegularExpressionsGenerator.GetDatePattern("-");
			string patternTime = LexicalAnalysis.RegularExpressionsGenerator.GetTimePattern(DigitsFsec);
			return new NegativeLookahead(affix, @$"\{{\s*((t|T)(s|S))\s*'{patternsDate.Item1}", @$"\{{\s*((t|T)(s|S))\s*'{patternsDate.Item2} {patternTime}'\s*\}}", false);
		}
	}
}
