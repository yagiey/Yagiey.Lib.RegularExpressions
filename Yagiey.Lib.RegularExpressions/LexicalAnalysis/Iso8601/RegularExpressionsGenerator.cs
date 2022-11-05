using System;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.Iso8601
{
	using DFA = IDeterministicFiniteAutomaton<char>;

	public class RegularExpressionsGenerator
	{
		public static DFA Date()
		{
			Tuple<string, string> patterns = GetDatePattern();
			return new NegativeLookahead(patterns.Item1, patterns.Item2, false);
		}

		public static DFA Date(StringAffix affix)
		{
			Tuple<string, string> patterns = GetDatePattern();
			return new NegativeLookahead(affix, patterns.Item1, patterns.Item2, false);
		}

		public static Tuple<string, string> GetDatePattern()
		{
			const string parts01M = @"(02)";
			const string parts01D = @"(29)";
			const string parts01Y = @"((0|2|4|6|8)(1|2|3|5|6|7|9)|(1|3|5|7|9)(0|1|3|4|5|7|8|9))00";

			const string PatternYYYY1 = @"((1|2|3|4|5|6|7|8|9)\d\d\d|0(1|2|3|4|5|6|7|8|9)\d\d|00(1|2|3|4|5|6|7|8|9)\d|000(1|2|3|4|5|6|7|8|9))";

			const string parts02M = @"(01|03|05|07|08|10|12)";
			const string parts02D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)\d|3(0|1))";
			const string parts02Y = PatternYYYY1;

			const string parts03M = @"(04|06|09|11)";
			const string parts03D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)\d|30)";
			const string parts03Y = PatternYYYY1;

			const string parts04M = @"(02)";
			const string parts04D = @"(0(1|2|3|4|5|6|7|8|9)|1\d|2(0|1|2|3|4|5|6|7|8))";
			const string parts04Y = PatternYYYY1;

			const string parts05M = @"(02)";
			const string parts05D = @"(29)";
			const string parts05Y = @"((((1|2|3|4|5|6|7|8|9)\d|0(1|2|3|4|5|6|7|8|9))(0|2|4|6|8)|00(2|4|6|8))(0|4|8)|((((1|2|3|4|5|6|7|8|9)\d|0(1|2|3|4|5|6|7|8|9))(1|3|5|7|9)|00(1|3|5|7|9))(2|6)|000(4|8)))";

			const string separator = @"-";
			return Tuple.Create($"({parts01Y}{separator}{parts01M}{separator}{parts01D})", $"(({parts02Y}{separator}{parts02M}{separator}{parts02D})|({parts03Y}{separator}{parts03M}{separator}{parts03D})|({parts04Y}{separator}{parts04M}{separator}{parts04D})|({parts05Y}{separator}{parts05M}{separator}{parts05D}))");
		}

		public static DFA Time()
		{
			const int DigitsFsec = 9;
			string pattern = GetTimePattern(DigitsFsec);
			return new RegularExpression(pattern, false);
		}

		public static DFA Time(StringAffix affix)
		{
			const int DigitsFsec = 9;
			string pattern = GetTimePattern(DigitsFsec);
			string prefix = Constants.Sanitize(affix.Prefix);
			string suffix = Constants.Sanitize(affix.Suffix);
			return new RegularExpression($"{prefix}({pattern}){suffix}", false);
		}

		public static string GetTimePattern(int digitsFsec)
		{
			if (digitsFsec <= 0 || 10 <= digitsFsec)
			{
				const string ErrMsg = @"number of digits of fractional seconds must be between 1 and 9.";
				throw new ArgumentOutOfRangeException(nameof(digitsFsec), ErrMsg);
			}

			const string HH = @"((0|1)\d|2(0|1|2|3))";
			const string MM = @"(0|1|2|3|4|5)\d";
			const string SS = @"(0|1|2|3|4|5)\d";

			var listOr = Enumerable.Range(1, digitsFsec).Select(it => string.Join("", Enumerable.Repeat(@"\d", it)));
			string fractionalSeconds = @$"({string.Join("|", listOr)})";

			const string sep = @":";
			return $@"({HH}{sep}{MM}({sep}{SS}(\.{fractionalSeconds})?)?)";
		}

		public static DFA DateTime()
		{
			const int DigitsFsec = 9;
			var patternsDate = GetDatePattern();
			string patternTime = GetTimePattern(DigitsFsec);
			return new NegativeLookahead(patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}", false);
		}

		public static DFA DateTime(StringAffix affix)
		{
			const int DigitsFsec = 9;
			var patternsDate = GetDatePattern();
			string patternTime = GetTimePattern(DigitsFsec);
			return new NegativeLookahead(affix, patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}", false);
		}

		public static DFA DateTimeOffset()
		{
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn][{+|-}hh:mm]
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn]Z (UTC)
			const int DigitsFsec = 9;
			var patternsDate = GetDatePattern();
			string patternTime = GetTimePattern(DigitsFsec);
			string patternOffset = GetOffsetPattern();
			return new NegativeLookahead(patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}{patternOffset}", false);
		}

		public static DFA DateTimeOffset(StringAffix affix)
		{
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn][{+|-}hh:mm]
			// YYYY-MM-DDThh:mm:ss[.nnnnnnn]Z (UTC)
			const int DigitsFsec = 9;
			var patternsDate = GetDatePattern();
			string patternTime = GetTimePattern(DigitsFsec);
			string patternOffset = GetOffsetPattern();
			return new NegativeLookahead(affix, patternsDate.Item1, $"{patternsDate.Item2}T{patternTime}{patternOffset}", false);
		}

		public static string GetOffsetPattern()
		{
			// 00:00-13:59,14:00
			const string HHMM1 = @$"(0\d|1(0|1|2|3)):(0|1|2|3|4|5)\d";
			const string HHMM2 = @$"14:00";
			const string Offset1 = @$"(\+|-)(({HHMM1})|{HHMM2})";
			const string Offset2 = @"(Z|z)";
			const string Offset = $@"(({Offset1})|({Offset2}))";
			return Offset;
		}
	}
}
