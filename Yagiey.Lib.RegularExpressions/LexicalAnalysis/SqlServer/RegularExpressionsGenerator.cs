using System;
using System.Collections.Generic;
using System.Linq;
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

		public static DFA UnicodeStringLiteral()
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

			string negativeHead = '(' + string.Join("|", patterns.Select(it => it.Item1)) + ')';
			string main = '(' + string.Join("|", patterns.Select(it => it.Item2)) + ')';

			return new NegativeLookahead(negativeHead, main, false);
		}

		private static Tuple<string, string>[] GetDatePattern(DateNumericMonthFormatEnum dateFormatEnum, DateSeparatorEnum separatorEnum)
		{
			if (separatorEnum == DateSeparatorEnum.None)
			{
				const string ErrMsg = @"Date format must not be None.";
				throw new ArgumentException(ErrMsg, nameof(dateFormatEnum));
			}

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

			List<string> separators = new List<string>();
			if ((separatorEnum & DateSeparatorEnum.Slash) == DateSeparatorEnum.Slash)
			{
				separators.Add(@"/");
			}
			if ((separatorEnum & DateSeparatorEnum.Dot) == DateSeparatorEnum.Dot)
			{
				separators.Add(@"\.");
			}
			if ((separatorEnum & DateSeparatorEnum.Dash) == DateSeparatorEnum.Dash)
			{
				separators.Add(@"-");
			}

			switch (dateFormatEnum)
			{
				case DateNumericMonthFormatEnum.Mdy:
					{
						return
							separators
							.Select(separator => Tuple.Create($"({parts01M}{separator}{parts01D}{separator}{parts01Y})", $"(({parts02M}{separator}{parts02D}{separator}{parts02Y})|({parts03M}{separator}{parts03D}{separator}{parts03Y})|({parts04M}{separator}{parts04D}{separator}{parts04Y})|({parts05M}{separator}{parts05D}{separator}{parts05Y}))"))
							.ToArray();
					}
				case DateNumericMonthFormatEnum.Myd:
					{
						return
							separators
							.Select(separator => Tuple.Create($"({parts01M}{separator}{parts01Y}{separator}{parts01D})", $"(({parts02M}{separator}{parts02Y}{separator}{parts02D})|({parts03M}{separator}{parts03Y}{separator}{parts03D})|({parts04M}{separator}{parts04Y}{separator}{parts04D})|({parts05M}{separator}{parts05Y}{separator}{parts05D}))"))
							.ToArray();
					}
				case DateNumericMonthFormatEnum.Dmy:
					{
						return
							separators
							.Select(separator => Tuple.Create($"({parts01D}{separator}{parts01M}{separator}{parts01Y})", $"(({parts02D}{separator}{parts02M}{separator}{parts02Y})|({parts03D}{separator}{parts03M}{separator}{parts03Y})|({parts04D}{separator}{parts04M}{separator}{parts04Y})|({parts05D}{separator}{parts05M}{separator}{parts05Y}))"))
							.ToArray();
					}
				case DateNumericMonthFormatEnum.Dym:
					{
						return
							separators
							.Select(separator => Tuple.Create($"({parts01D}{separator}{parts01Y}{separator}{parts01M})", $"(({parts02D}{separator}{parts02Y}{separator}{parts02M})|({parts03D}{separator}{parts03Y}{separator}{parts03M})|({parts04D}{separator}{parts04Y}{separator}{parts04M})|({parts05D}{separator}{parts05Y}{separator}{parts05M}))"))
							.ToArray();
					}
				case DateNumericMonthFormatEnum.Ymd:
				default:
					{
						return
							separators
							.Select(separator => Tuple.Create($"({parts01Y}{separator}{parts01M}{separator}{parts01D})", $"(({parts02Y}{separator}{parts02M}{separator}{parts02D})|({parts03Y}{separator}{parts03M}{separator}{parts03D})|({parts04Y}{separator}{parts04M}{separator}{parts04D})|({parts05Y}{separator}{parts05M}{separator}{parts05D}))"))
							.ToArray();
					}
			}
		}
	}
}
