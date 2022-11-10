using System;
using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.SqlServer
{
	using DFA = IDeterministicFiniteAutomaton<char>;
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

	public class RegularExpressionsGenerator
	{
		public static DFA StringLiteral()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0 ,
					new Dictionary<IInput, int> {
						{ new Input('\''), 1 },
					}
				},
				{
					1 ,
					new Dictionary<IInput, int> {
						{ new InputWithPredicate(new Not<char>(new Equal<char>('\''))), 1 },
						{ new Input('\''), 2 },
					}
				},
				{
					2 ,
					new Dictionary<IInput, int> {
						{ new Input('\''), 1 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 2 }, transitionMap, false);
		}

		public static DFA StringLiteralUnicode()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0 ,
					new Dictionary<IInput, int> {
						{ new Input('N'), 1 },
					}
				},
				{
					1 ,
					new Dictionary<IInput, int> {
						{ new Input('\''), 2 },
					}
				},
				{
					2 ,
					new Dictionary<IInput, int> {
						{ new InputWithPredicate(new Not<char>(new Equal<char>('\''))), 2 },
						{ new Input('\''), 3 },
					}
				},
				{
					3 ,
					new Dictionary<IInput, int> {
						{ new Input('\''), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 3 }, transitionMap, false);
		}

		public static DFA DateNumericMonth(DateNumericMonthFormatEnum dateFormatEnum, DateSeparatorEnum dateSeparatorEnum)
		{
			var patterns = GetDatePattern(dateFormatEnum, dateSeparatorEnum);
			return new NegativeLookahead(patterns.Item1, patterns.Item2, false);
		}

		public static DFA DateNumericMonth(StringAffix affix, DateNumericMonthFormatEnum dateFormatEnum, DateSeparatorEnum dateSeparatorEnum)
		{
			var patterns = GetDatePattern(dateFormatEnum, dateSeparatorEnum);
			return new NegativeLookahead(affix, patterns.Item1, patterns.Item2, false);
		}

		public static Tuple<string, string> GetDatePattern(DateNumericMonthFormatEnum dateFormatEnum, DateSeparatorEnum dateSeparatorEnum)
		{
			const string parts01M = @"(2|02)";
			const string parts01D = @"(29)";
			const string parts01Y = @"((0|2|4|6|8)(1|2|3|5|6|7|9)|(1|3|5|7|9)(0|1|3|4|5|7|8|9))00";

			const string PatternYYYY1 = @"((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|00(1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|000(1|2|3|4|5|6|7|8|9))";

			const string parts02M = @"(1|3|5|7|8|01|03|05|07|08|10|12)";
			const string parts02D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)(0|1|2|3|4|5|6|7|8|9)|3(0|1))";
			const string parts02Y = PatternYYYY1;

			const string parts03M = @"(4|6|9|04|06|09|11)";
			const string parts03D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)(0|1|2|3|4|5|6|7|8|9)|30)";
			const string parts03Y = PatternYYYY1;

			const string parts04M = @"(2|02)";
			const string parts04D = @"(0(1|2|3|4|5|6|7|8|9)|1(0|1|2|3|4|5|6|7|8|9)|2(0|1|2|3|4|5|6|7|8))";
			const string parts04Y = PatternYYYY1;

			const string parts05M = @"(2|02)";
			const string parts05D = @"(29)";
			const string parts05Y = @"((((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9))(0|2|4|6|8)|00(2|4|6|8))(0|4|8)|((((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9))(1|3|5|7|9)|00(1|3|5|7|9))(2|6)|000(4|8)))";

			string separator = dateSeparatorEnum switch
			{
				DateSeparatorEnum.Slash => @"/",
				DateSeparatorEnum.Dot => @"\.",
				_ => @"-",
			};

			switch (dateFormatEnum)
			{
				case DateNumericMonthFormatEnum.Mdy:
					{
						return
							Tuple.Create($"({parts01M}{separator}{parts01D}{separator}{parts01Y})", $"(({parts02M}{separator}{parts02D}{separator}{parts02Y})|({parts03M}{separator}{parts03D}{separator}{parts03Y})|({parts04M}{separator}{parts04D}{separator}{parts04Y})|({parts05M}{separator}{parts05D}{separator}{parts05Y}))");
					}
				case DateNumericMonthFormatEnum.Myd:
					{
						return
							Tuple.Create($"({parts01M}{separator}{parts01Y}{separator}{parts01D})", $"(({parts02M}{separator}{parts02Y}{separator}{parts02D})|({parts03M}{separator}{parts03Y}{separator}{parts03D})|({parts04M}{separator}{parts04Y}{separator}{parts04D})|({parts05M}{separator}{parts05Y}{separator}{parts05D}))");
					}
				case DateNumericMonthFormatEnum.Dmy:
					{
						return
							Tuple.Create($"({parts01D}{separator}{parts01M}{separator}{parts01Y})", $"(({parts02D}{separator}{parts02M}{separator}{parts02Y})|({parts03D}{separator}{parts03M}{separator}{parts03Y})|({parts04D}{separator}{parts04M}{separator}{parts04Y})|({parts05D}{separator}{parts05M}{separator}{parts05Y}))");
					}
				case DateNumericMonthFormatEnum.Dym:
					{
						return
							Tuple.Create($"({parts01D}{separator}{parts01Y}{separator}{parts01M})", $"(({parts02D}{separator}{parts02Y}{separator}{parts02M})|({parts03D}{separator}{parts03Y}{separator}{parts03M})|({parts04D}{separator}{parts04Y}{separator}{parts04M})|({parts05D}{separator}{parts05Y}{separator}{parts05M}))");
					}
				case DateNumericMonthFormatEnum.Ymd:
				default:
					{
						return
							Tuple.Create($"({parts01Y}{separator}{parts01M}{separator}{parts01D})", $"(({parts02Y}{separator}{parts02M}{separator}{parts02D})|({parts03Y}{separator}{parts03M}{separator}{parts03D})|({parts04Y}{separator}{parts04M}{separator}{parts04D})|({parts05Y}{separator}{parts05M}{separator}{parts05D}))");
					}
			}
		}

		public static DFA DateAlphabeticMonth(DateAlphabeticMonthFormatEnum dateFormatEnum)
		{
			var patterns = GetDatePattern(dateFormatEnum);
			return new NegativeLookahead(patterns.Item1, patterns.Item2, false);
		}

		public static DFA DateAlphabeticMonth(StringAffix affix, DateAlphabeticMonthFormatEnum dateFormatEnum)
		{
			var patterns = GetDatePattern(dateFormatEnum);
			return new NegativeLookahead(affix, patterns.Item1, patterns.Item2, false);
		}

		public static Tuple<string, string> GetDatePattern(DateAlphabeticMonthFormatEnum dateFormatEnum)
		{
			const string parts01M = @"(February|Feb\.)";
			const string parts01D = @"(29)";
			const string parts01Y = @"((0|2|4|6|8)(1|2|3|5|6|7|9)|(1|3|5|7|9)(0|1|3|4|5|7|8|9))00";

			const string PatternYYYY1 = @"((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|00(1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|000(1|2|3|4|5|6|7|8|9))";

			const string parts02M = @"(January|Jan\.|March|Mar\.|May|May\.|July|Jul\.|August|Aug\.|October|Oct\.|December|Dec\.)";
			const string parts02D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)(0|1|2|3|4|5|6|7|8|9)|3(0|1))";
			const string parts02Y = PatternYYYY1;

			const string parts03M = @"(April|Apr\.|June|Jun\.|September|Sep\.|November|Nov\.)";
			const string parts03D = @"(0(1|2|3|4|5|6|7|8|9)|(1|2)(0|1|2|3|4|5|6|7|8|9)|30)";
			const string parts03Y = PatternYYYY1;

			const string parts04M = @"(February|Feb\.)";
			const string parts04D = @"(0(1|2|3|4|5|6|7|8|9)|1(0|1|2|3|4|5|6|7|8|9)|2(0|1|2|3|4|5|6|7|8))";
			const string parts04Y = PatternYYYY1;

			const string parts05M = @"(February|Feb\.)";
			const string parts05D = @"(29)";
			const string parts05Y = @"((((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9))(0|2|4|6|8)|00(2|4|6|8))(0|4|8)|((((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|0(1|2|3|4|5|6|7|8|9))(1|3|5|7|9)|00(1|3|5|7|9))(2|6)|000(4|8)))";

			switch (dateFormatEnum)
			{
				// mon dd[,] yyyy
				case DateAlphabeticMonthFormatEnum.Mdy:
					{
						return
							Tuple.Create(@$"({parts01M} {parts01D},? {parts01Y})", @$"(({parts02M} {parts02D},? {parts02Y})|({parts03M} {parts03D},? {parts03Y})|({parts04M} {parts04D},? {parts04Y})|({parts05M} {parts05D},? {parts05Y}))");
					}

				// mon yyyy dd
				case DateAlphabeticMonthFormatEnum.Myd:
					{
						return
							Tuple.Create(@$"({parts01M} {parts01Y} {parts01D})", @$"(({parts02M} {parts02Y} {parts02D})|({parts03M} {parts03Y} {parts03D})|({parts04M} {parts04Y} {parts04D})|({parts05M} {parts05Y} {parts05D}))");
					}

				// dd mon[,] yyyy
				case DateAlphabeticMonthFormatEnum.Dmy:
					{
						return
							Tuple.Create(@$"({parts01D} {parts01M},? {parts01Y})", @$"(({parts02D} {parts02M},? {parts02Y})|({parts03D} {parts03M},? {parts03Y})|({parts04D} {parts04M},? {parts04Y})|({parts05D} {parts05M},? {parts05Y}))");
					}

				// dd yyyy mon
				case DateAlphabeticMonthFormatEnum.Dym:
					{
						return
							Tuple.Create(@$"({parts01D} {parts01Y} {parts01M})", @$"(({parts02D} {parts02Y} {parts02M})|({parts03D} {parts03Y} {parts03M})|({parts04D} {parts04Y} {parts04M})|({parts05D} {parts05Y} {parts05M}))");
					}

				// yyyy mon dd
				case DateAlphabeticMonthFormatEnum.Ymd:
				default:
					{
						return
							Tuple.Create(@$"({parts01Y} {parts01M} {parts01D})", @$"(({parts02Y} {parts02M} {parts02D})|({parts03Y} {parts03M} {parts03D})|({parts04Y} {parts04M} {parts04D})|({parts05Y} {parts05M} {parts05D}))");
					}

				// yyyy dd mon
				case DateAlphabeticMonthFormatEnum.Ydm:
					{
						return
							Tuple.Create(@$"({parts01Y} {parts01D} {parts01M})", @$"(({parts02Y} {parts02D} {parts02M})|({parts03Y} {parts03D} {parts03M})|({parts04Y} {parts04D} {parts04M})|({parts05Y} {parts05D} {parts05M}))");
					}
			}
		}

		public static DFA Time()
		{
			string pattern = GetTimePattern();
			return new RegularExpression(pattern, false);
		}

		public static DFA Time(StringAffix affix)
		{
			string pattern = GetTimePattern();
			string prefix = Constants.Sanitize(affix.Prefix);
			string suffix = Constants.Sanitize(affix.Suffix);
			return new RegularExpression($"{prefix}({pattern}){suffix}", false);
		}

		public static string GetTimePattern()
		{
			// hhAM[PM]
			// hh AM[PM]
			const string pat1 = @"00( ?(A|a)(M|m))?";
			const string pat2 = @"(01|02|03|04|05|06|07|08|09|10|11)( ?(A|a|P|p)(M|m))?";
			const string pat3 = @"12( ?(A|a|P|p)(M|m))?";
			const string pat4 = @"(13|14|15|16|17|18|19|20|21|22|23)( ?(P|p)(M|m))?";
			string patHhOnly = $"{pat1}|{pat2}|{pat3}|{pat4}";

			// hh: mm[:ss][:fractional seconds][AM][PM]
			// hh: mm[:ss][.fractional seconds][AM][PM]
			const string pat5 = @"00:(0|1|2|3|4|5)\d(:(0|1|2|3|4|5)\d((:|\.)(\d|\d\d|\d\d\d))?)?((A|a)(M|m))?";
			const string pat6 = @"(01|02|03|04|05|06|07|08|09|10|11):(0|1|2|3|4|5)\d(:(0|1|2|3|4|5)\d((:|\.)(\d|\d\d|\d\d\d))?)?((A|a|P|p)(M|m))?";
			const string pat7 = @"12:(0|1|2|3|4|5)\d(:(0|1|2|3|4|5)\d((:|\.)(\d|\d\d|\d\d\d))?)?((A|a|P|p)(M|m))?";
			const string pat8 = @"(13|14|15|16|17|18|19|20|21|22|23):(0|1|2|3|4|5)\d(:(0|1|2|3|4|5)\d((:|\.)(\d|\d\d|\d\d\d))?)?((P|p)(M|m))?";
			string patLongStyle = $"{pat5}|{pat6}|{pat7}|{pat8}";

			return $"({patHhOnly}|{patLongStyle})";
		}
	}
}
