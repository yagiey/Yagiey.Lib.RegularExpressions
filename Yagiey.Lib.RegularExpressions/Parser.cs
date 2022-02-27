using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<Input, IEnumerable<int>>>;
	using StartAndEnd = Tuple<int, int>;

	/// <summary>
	/// parser for regular expression
	/// </summary>
	/// <definition>
	/// expression = level2, { vertical_bar, level2 }
	/// level2 = level1, { level1 }
	/// level1 = level0 | level0, question | level0, asterisk
	/// level0 = all_characters | lparen, expression, rparen
	/// 
	/// all_characters = __all_characters__except(spaceial_characters) | escape
	/// escape = backslash, spaceial_characters
	/// spaceial_characters = backslash | vertical_bar | asterisk | lparen | rparen | question | dot
	/// 
	/// backslash = '\'
	/// vertical_bar = '|'
	/// asterisk = '*'
	/// lparen = '('
	/// rparen = ')'
	/// question = '?'
	/// dot = '.'
	/// </definition>
	internal class Parser
	{
		public NFA EpsilonNFA
		{
			get;
			private set;
		}

		public Parser(string source)
		{
			var lexer = new LexicalAnalyzer(source);
			var itorToken = lexer.GetEnumerator();

			var ret = GetExpression(itorToken);

			int startNode = ret.Item1.Item1;
			IEnumerable<int> acceptingNodeSet = new int[] { ret.Item1.Item2 };
			NFATransitionMap transitionMap = ret.Item2;

			EpsilonNFA = new NFA(startNode, acceptingNodeSet, transitionMap);
		}

		#region parsing

		private static Tuple<StartAndEnd, NFATransitionMap> GetExpression(IEnumerator<Tuple<char, char?[]>> itorToken)
		{
			NFATransitionMap transitionMap = new Dictionary<int, IDictionary<Input, IEnumerable<int>>>();
			var itorID = new SequenceNumberEnumerator(0);
			StartAndEnd result = GetExpression(transitionMap, itorToken, itorID);
			return Tuple.Create(result, transitionMap);
		}

		private static StartAndEnd GetExpression(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv2 = GetLevel2(transitionMap, itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfSelection())
			{
				List<Tuple<int, Input, int[]>> t = new();

				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;

				t.Add(Tuple.Create(start, Input.Empty, new int[] { lv2.Item1 }));
				t.Add(Tuple.Create(lv2.Item2, Input.Empty, new int[] { end }));

				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				do
				{
					var lv2New = GetLevel2(transitionMap, itorToken, itorID);
					t.Add(Tuple.Create(start, Input.Empty, new int[] { lv2New.Item1 }));
					t.Add(Tuple.Create(lv2New.Item2, Input.Empty, new int[] { end }));

					if (!itorToken.Current.CanNextBeRegardedAsRestOfSelection())
					{
						break;
					}

					if (!itorToken.MoveNext())
					{
						throw new Exception();
					}
				} while (true);

				foreach (var item in t)
				{
					NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
				}
				return new StartAndEnd(start, end);
			}
			else
			{
				return lv2;
			}
		}

		private static StartAndEnd GetLevel2(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv1 = GetLevel1(transitionMap, itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
			{
				List<Tuple<int, Input, int[]>> t = new();

				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;
				t.Add(Tuple.Create(start, Input.Empty, new int[] { lv1.Item1 }));

				int prevEnd = lv1.Item2;
				do
				{
					var lv1New = GetLevel1(transitionMap, itorToken, itorID);
					t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { lv1New.Item1 }));
					prevEnd = lv1New.Item2;

					if (!itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
					{
						break;
					}
				} while (true);

				t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { end }));

				foreach (var item in t)
				{
					NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
				}
				return new StartAndEnd(start, end);
			}
			else
			{
				return lv1;
			}
		}

		private static StartAndEnd GetLevel1(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv0 = GetLevel0(transitionMap, itorToken, itorID);

			char? next = itorToken.Current.Item2[0];
			if (next.HasValue && (next == Constants.Asterisk || next == Constants.Question))
			{
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				int start;
				int end;
				if (next == Constants.Asterisk)
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					int node1 = itorID.Current;
					itorID.MoveNext();
					int node2 = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					var t1 = Tuple.Create(start, Input.Empty, new int[] { node1, end });
					var t2 = Tuple.Create(node1, Input.Empty, new int[] { lv0.Item1 });
					var t3 = Tuple.Create(lv0.Item2, Input.Empty, new int[] { node2 });
					var t4 = Tuple.Create(node2, Input.Empty, new int[] { node1, end });

					NFA.AddTransition(transitionMap, t1.Item1, t1.Item2, t1.Item3);
					NFA.AddTransition(transitionMap, t2.Item1, t2.Item2, t2.Item3);
					NFA.AddTransition(transitionMap, t3.Item1, t3.Item2, t3.Item3);
					NFA.AddTransition(transitionMap, t4.Item1, t4.Item2, t4.Item3);
				}
				else
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					var t1 = Tuple.Create(start, Input.Empty, new int[] { lv0.Item1 });
					var t2 = Tuple.Create(lv0.Item2, Input.Empty, new int[] { end });
					var t3 = Tuple.Create(start, Input.Empty, new int[] { end });

					NFA.AddTransition(transitionMap, t1.Item1, t1.Item2, t1.Item3);
					NFA.AddTransition(transitionMap, t2.Item1, t2.Item2, t2.Item3);
					NFA.AddTransition(transitionMap, t3.Item1, t3.Item2, t3.Item3);
				}

				return new StartAndEnd(start, end);
			}
			else
			{
				return lv0;
			}
		}

		private static StartAndEnd GetLevel0(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			if (!itorToken.MoveNext())
			{
				throw new Exception();
			}

			if (itorToken.Current.IsBeginingOfExpressionWithParen())
			{
				var ret = GetExpression(transitionMap, itorToken, itorID);

				if (!itorToken.Current.IsNextEndOfExpressionWithParen())
				{
					throw new Exception();
				}
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				return ret;
			}
			else if (itorToken.Current.IsBeginingOfCharacterClassWithBracket())
			{
				var ret = GetCharacterClass(transitionMap, itorToken, itorID);

				if (!itorToken.Current.IsNextEndOfCharacterClassWithBracket())
				{
					throw new Exception();
				}
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				return ret;
			}
			else
			{
				char current = itorToken.Current.Item1;
				char? next = itorToken.Current.Item2[0];

				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;

				if (current == Constants.Backslash && next.HasValue && Constants.EscapedChar.Any(_ => _ == next.Value))
				{
					itorToken.MoveNext();

					char nextChar = next.Value;
					IEnumerable<char> characters;
					if (nextChar == Constants.EscapedTab)
					{
						characters = new char[] { Constants.Tab };
					}
					else if (nextChar == Constants.EscapedCr)
					{
						characters = new char[] { Constants.CarriageReturn };
					}
					else if (nextChar == Constants.EscapedLf)
					{
						characters = new char[] { Constants.LineFeed };
					}
					else if (nextChar == Constants.EscapedDigit)
					{
						characters = Enumerable.Range(0, 10).Select(n => Convert.ToChar('0' + n));
					}
					else if (nextChar == Constants.EscapedWhiteSpace)
					{
						characters = new char[] { Constants.Tab, Constants.CarriageReturn, Constants.LineFeed, Constants.Space };
					}
					else
					{
						characters = Enumerable.Empty<char>()
							.Concat(Enumerable.Range(0, 26).Select(n => Convert.ToChar('a' + n)))
							.Concat(Enumerable.Range(0, 26).Select(n => Convert.ToChar('A' + n)))
							.Concat(Enumerable.Range(0, 10).Select(n => Convert.ToChar('0' + n)))
							.Append(Constants.Underline);
					}
					AddTransitions(transitionMap, start, characters, new int[] { end });
				}
				else
				{
					Input input;
					if (current == Constants.Dot)
					{
						input = new Input(InputType.Negative, Constants.LineFeed);
					}
					else if (current == Constants.Backslash)
					{
						if (!itorToken.MoveNext())
						{
							const string errMsg = "insufficient escaped char";
							throw new Exception(errMsg);
						}
						input = new Input(itorToken.Current.Item1);
					}
					else
					{
						input = new Input(current);
					}
					NFA.AddTransition(transitionMap, start, input, new int[] { end });
				}

				return new StartAndEnd(start, end);
			}
		}

		private static StartAndEnd GetCharacterClass(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			itorID.MoveNext();
			int start = itorID.Current;
			itorID.MoveNext();
			int end = itorID.Current;

			bool isFirst = true;
			while (true)
			{
				char? next = itorToken.Current.Item2[0];

				if (next == null)
				{
					const string ErrMsg = "unclosed character class";
					throw new Exception(ErrMsg);
				}

				if (isFirst && itorToken.Current.IsNextEndOfCharacterClassWithBracket())
				{
					const string ErrMsg = "empty character class";
					throw new Exception(ErrMsg);
				}

				if (itorToken.Current.IsNextEndOfCharacterClassWithBracket())
				{
					break;
				}

				itorToken.MoveNext();

				char current = itorToken.Current.Item1;
				Input input = new Input(current);
				NFA.AddTransition(transitionMap, start, input, new int[] { end });
				isFirst = false;
			}
			return new StartAndEnd(start, end);
		}

		public static NFATransitionMap AddTransitions(NFATransitionMap transitionMap, int start, IEnumerable<char> characters, IEnumerable<int> ends)
		{
			foreach (char ch in characters)
			{
				Input input = new Input(ch);
				NFA.AddTransition(transitionMap, start, input, ends);
			}
			return transitionMap;
		}

		#endregion
	}
}
