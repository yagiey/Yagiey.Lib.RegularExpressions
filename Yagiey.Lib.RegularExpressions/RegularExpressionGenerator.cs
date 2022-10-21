using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions
{
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

	public static class RegularExpressionGenerator
	{
		public static RegularExpression GenerateString(string str, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(str))
			{
				const string ErrMsg = "string must not be null or empty.";
				throw new ArgumentException(ErrMsg, nameof(str));
			}

			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>();
			for (int i = 0; i < str.Length; i++)
			{
				transitionMap.Add(
					i,
					new Dictionary<IInput, int> { { new Input(str[i]), i + 1 } }
				);
			}
			return new RegularExpression(0, new int[] { str.Length }, transitionMap, ignoreCase);
		}

		public static RegularExpression GenerateIdentifier()
		{
			//[a-zA-Z_]\w*

			IEnumerable<char> identifierHead =
				Enumerable.Empty<char>()
				.Concat(Enumerable.Range(0, 26).Select(it => Convert.ToChar('A' + it)))
				.Concat(Enumerable.Range(0, 26).Select(it => Convert.ToChar('a' + it)))
				.Append('_');

			IEnumerable<char> identifierChar =
				identifierHead
				.Concat(Enumerable.Range(0, 10).Select(it => Convert.ToChar('0' + it)));

			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int>(identifierHead.Select(it => KeyValuePair.Create((IInput)new Input(it), 1)))
				},
				{
					1,
					new Dictionary<IInput, int>(identifierChar.Select(it => KeyValuePair.Create((IInput)new Input(it), 1)))
				},
			};
			return new RegularExpression(0, new int[] { 1 }, transitionMap, false);
		}

		public static RegularExpression GenerateIntegerNumber()
		{
			//[+-]?([1-9]\d*|0)

			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int> {
						{ new Input('+'), 1 },
						{ new Input('-'), 1 },
						{ new Input('0'), 2 },
						{ new Input('1'), 3 },
						{ new Input('2'), 3 },
						{ new Input('3'), 3 },
						{ new Input('4'), 3 },
						{ new Input('5'), 3 },
						{ new Input('6'), 3 },
						{ new Input('7'), 3 },
						{ new Input('8'), 3 },
						{ new Input('9'), 3 },
					}
				},
				{
					1,
					new Dictionary<IInput, int> {
						{ new Input('0'), 2 },
						{ new Input('1'), 3 },
						{ new Input('2'), 3 },
						{ new Input('3'), 3 },
						{ new Input('4'), 3 },
						{ new Input('5'), 3 },
						{ new Input('6'), 3 },
						{ new Input('7'), 3 },
						{ new Input('8'), 3 },
						{ new Input('9'), 3 },
					}
				},
				{
					3,
					new Dictionary<IInput, int> {
						{ new Input('0'), 3 },
						{ new Input('1'), 3 },
						{ new Input('2'), 3 },
						{ new Input('3'), 3 },
						{ new Input('4'), 3 },
						{ new Input('5'), 3 },
						{ new Input('6'), 3 },
						{ new Input('7'), 3 },
						{ new Input('8'), 3 },
						{ new Input('9'), 3 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 2, 3 }, transitionMap, false);
		}

		public static RegularExpression GenerateRealNumber()
		{
			//(\-|\+)?(\d+(\.\d*)?|\.\d+)((e|E)(\-|\+)?\d+)?
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int> {
						{ new Input('+'), 1 },
						{ new Input('-'), 1 },
						{ new Input('.'), 2 },
						{ new Input('0'), 7 },
						{ new Input('1'), 7 },
						{ new Input('2'), 7 },
						{ new Input('3'), 7 },
						{ new Input('4'), 7 },
						{ new Input('5'), 7 },
						{ new Input('6'), 7 },
						{ new Input('7'), 7 },
						{ new Input('8'), 7 },
						{ new Input('9'), 7 },
					}
				},
				{
					1,
					new Dictionary<IInput, int> {
						{ new Input('.'), 2 },
						{ new Input('0'), 7 },
						{ new Input('1'), 7 },
						{ new Input('2'), 7 },
						{ new Input('3'), 7 },
						{ new Input('4'), 7 },
						{ new Input('5'), 7 },
						{ new Input('6'), 7 },
						{ new Input('7'), 7 },
						{ new Input('8'), 7 },
						{ new Input('9'), 7 },
					}
				},
				{
					2,
					new Dictionary<IInput, int> {
						{ new Input('0'), 5 },
						{ new Input('1'), 5 },
						{ new Input('2'), 5 },
						{ new Input('3'), 5 },
						{ new Input('4'), 5 },
						{ new Input('5'), 5 },
						{ new Input('6'), 5 },
						{ new Input('7'), 5 },
						{ new Input('8'), 5 },
						{ new Input('9'), 5 },
					}
				},
				{
					3,
					new Dictionary<IInput, int> {
						{ new Input('0'), 6 },
						{ new Input('1'), 6 },
						{ new Input('2'), 6 },
						{ new Input('3'), 6 },
						{ new Input('4'), 6 },
						{ new Input('5'), 6 },
						{ new Input('6'), 6 },
						{ new Input('7'), 6 },
						{ new Input('8'), 6 },
						{ new Input('9'), 6 },
					}
				},
				{
					4,
					new Dictionary<IInput, int> {
						{ new Input('+'), 3 },
						{ new Input('-'), 3 },
						{ new Input('0'), 6 },
						{ new Input('1'), 6 },
						{ new Input('2'), 6 },
						{ new Input('3'), 6 },
						{ new Input('4'), 6 },
						{ new Input('5'), 6 },
						{ new Input('6'), 6 },
						{ new Input('7'), 6 },
						{ new Input('8'), 6 },
						{ new Input('9'), 6 },
					}
				},
				{
					5,
					new Dictionary<IInput, int> {
						{ new Input('0'), 5 },
						{ new Input('1'), 5 },
						{ new Input('2'), 5 },
						{ new Input('3'), 5 },
						{ new Input('4'), 5 },
						{ new Input('5'), 5 },
						{ new Input('6'), 5 },
						{ new Input('7'), 5 },
						{ new Input('8'), 5 },
						{ new Input('9'), 5 },
						{ new Input('E'), 4 },
						{ new Input('e'), 4 },
					}
				},
				{
					6,
					new Dictionary<IInput, int> {
						{ new Input('0'), 6 },
						{ new Input('1'), 6 },
						{ new Input('2'), 6 },
						{ new Input('3'), 6 },
						{ new Input('4'), 6 },
						{ new Input('5'), 6 },
						{ new Input('6'), 6 },
						{ new Input('7'), 6 },
						{ new Input('8'), 6 },
						{ new Input('9'), 6 },
					}
				},
				{
					7,
					new Dictionary<IInput, int> {
						{ new Input('.'), 5 },
						{ new Input('0'), 7 },
						{ new Input('1'), 7 },
						{ new Input('2'), 7 },
						{ new Input('3'), 7 },
						{ new Input('4'), 7 },
						{ new Input('5'), 7 },
						{ new Input('6'), 7 },
						{ new Input('7'), 7 },
						{ new Input('8'), 7 },
						{ new Input('9'), 7 },
						{ new Input('E'), 4 },
						{ new Input('e'), 4 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 5, 6, 7 }, transitionMap, false);
		}

		public static RegularExpression GenerateCppStyleLineComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int> {
						{ new Input('/'), 1 },
					}
				},
				{
					1,
					new Dictionary<IInput, int> {
						{ new Input('/'), 2 },
					}
				},
				{
					2,
					new Dictionary<IInput, int> {
						{ new Input('\r'), 3 },
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(
							new And<char>(
								new List<IUnaryFunctionObject<bool,char>>
								{
									new Not<char>(new Equal<char>('\r')),
									new Not<char>(new Equal<char>('\n')),
								}
							)
						), 2 },
					}
				},
				{
					3,
					new Dictionary<IInput, int> {
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(new Not<char>(new Equal<char>('\n'))), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 4 }, transitionMap, false);
		}

		public static RegularExpression GenerateSqlStyleLineComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int> {
						{ new Input('-'), 1 },
					}
				},
				{
					1,
					new Dictionary<IInput, int> {
						{ new Input('-'), 2 },
					}
				},
				{
					2,
					new Dictionary<IInput, int> {
						{ new Input('\r'), 3 },
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(
							new And<char>(
								new List<IUnaryFunctionObject<bool,char>>
								{
									new Not<char>(new Equal<char>('\r')),
									new Not<char>(new Equal<char>('\n')),
								}
							)
						), 2 },
					}
				},
				{
					3,
					new Dictionary<IInput, int> {
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(new Not<char>(new Equal<char>('\n'))), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 4 }, transitionMap, false);
		}

		public static RegularExpression GenerateCStyleBlockComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0 ,
					new Dictionary<IInput, int> {
						{ new Input('/'), 1 },
					}
				},
				{
					1 ,
					new Dictionary<IInput, int> {
						{ new Input('*'), 2 },
					}
				},
				{
					2 ,
					new Dictionary<IInput, int> {
						{ new Input('*'), 3 },
						{ new InputWithPredicate(new Not<char>( new Equal<char>('*'))), 2 },
					}
				},
				{
					3 ,
					new Dictionary<IInput, int> {
						{ new Input('/'), 4 },
						{ new InputWithPredicate(new Not<char>( new Equal<char>('/'))), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 4 }, transitionMap, false);
		}
	}
}
