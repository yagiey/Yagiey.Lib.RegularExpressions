﻿using System;
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
		const char NewLine = '\n';

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

		private static Tuple<StartAndEnd, NFATransitionMap> GetExpression(IEnumerator<Tuple<Token, Token?[]>> itorToken)
		{
			NFATransitionMap transitionMap = new Dictionary<int, IDictionary<Input, IEnumerable<int>>>();
			var itorID = new SequenceNumberEnumerator(0);
			StartAndEnd result = GetExpression(transitionMap, itorToken, itorID);
			return Tuple.Create(result, transitionMap);
		}

		private static StartAndEnd GetExpression(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv2 = GetLevel2(transitionMap, itorToken, itorID);
			int start1st = lv2.Item1;
			int end1st = lv2.Item2;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && next.CanBeRegardedAsRestOfSelection())
			{
				List<Tuple<int, Input, int[]>> t = new();

				itorID.MoveNext();
				int s = itorID.Current;
				itorID.MoveNext();
				int e = itorID.Current;

				t.Add(Tuple.Create(s, Input.Empty, new int[] { start1st }));
				t.Add(Tuple.Create(end1st, Input.Empty, new int[] { e }));

				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				do
				{
					var lv2New = GetLevel2(transitionMap, itorToken, itorID);
					int start2nd = lv2New.Item1;
					int end2nd = lv2New.Item2;
					t.Add(Tuple.Create(s, Input.Empty, new int[] { start2nd }));
					t.Add(Tuple.Create(end2nd, Input.Empty, new int[] { e }));

					next = itorToken.Current.Item2[0];
					if (next == null || !next.CanBeRegardedAsRestOfSelection())
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
				start1st = s;
				end1st = e;
			}

			return new StartAndEnd(start1st, end1st);
		}

		private static StartAndEnd GetLevel2(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv1 = GetLevel1(transitionMap, itorToken, itorID);
			int start1st = lv1.Item1;
			int end1st = lv1.Item2;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && next.CanBeRegardedAsRestOfConcatenation())
			{
				List<Tuple<int, Input, int[]>> t = new();

				itorID.MoveNext();
				int s = itorID.Current;
				itorID.MoveNext();
				int e = itorID.Current;
				t.Add(Tuple.Create(s, Input.Empty, new int[] { start1st }));

				int prevEnd = end1st;
				do
				{
					var lv1New = GetLevel1(transitionMap, itorToken, itorID);
					int start2nd = lv1New.Item1;
					int end2nd = lv1New.Item2;

					t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { start2nd }));
					prevEnd = end2nd;

					next = itorToken.Current.Item2[0];
					if (next == null || !next.CanBeRegardedAsRestOfConcatenation())
					{
						break;
					}
				} while (true);

				t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { e }));

				foreach (var item in t)
				{
					NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
				}
				start1st = s;
				end1st = e;
			}

			return new StartAndEnd(start1st, end1st);
		}

		private static StartAndEnd GetLevel1(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv0 = GetLevel0(transitionMap, itorToken, itorID);
			int inStart = lv0.Item1;
			int inEnd = lv0.Item2;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && next.TokenType == TokenType.Character && (next.Source[0] == Constants.Asterisk || next.Source[0] == Constants.Question))
			{
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				int start;
				int end;
				if (next.Source[0] == Constants.Asterisk)
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
					var t2 = Tuple.Create(node1, Input.Empty, new int[] { inStart });
					var t3 = Tuple.Create(inEnd, Input.Empty, new int[] { node2 });
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

					var t1 = Tuple.Create(start, Input.Empty, new int[] { inStart });
					var t2 = Tuple.Create(inEnd, Input.Empty, new int[] { end });
					var t3 = Tuple.Create(start, Input.Empty, new int[] { end });

					NFA.AddTransition(transitionMap, t1.Item1, t1.Item2, t1.Item3);
					NFA.AddTransition(transitionMap, t2.Item1, t2.Item2, t2.Item3);
					NFA.AddTransition(transitionMap, t3.Item1, t3.Item2, t3.Item3);
				}

				return new StartAndEnd(start, end);
			}
			else
			{
				return new StartAndEnd(inStart, inEnd);
			}
		}

		private static StartAndEnd GetLevel0(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			if (!itorToken.MoveNext() || itorToken.Current == null)
			{
				throw new Exception();
			}

			Token curr = itorToken.Current.Item1;
			if (curr.IsBeginingOfExpressionWithParen())
			{
				var ret = GetExpression(transitionMap, itorToken, itorID);

				Token? next = itorToken.Current.Item2[0];
				if (next == null || !next.IsEndOfExpressionWithParen())
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
				char ch = curr.Source[0];
				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;

				if (curr.TokenType == TokenType.Escape && ch == 'd')
				{
					IEnumerable<char> characters = Enumerable.Range(0, 10).Select(n => Convert.ToChar('0' + n));
					AddTransitions(transitionMap, start, characters, new int[] { end });
				}
				else
				{
					Input input;
					if (curr.TokenType == TokenType.Character && ch == Constants.Dot)
					{
						input = new Input(InputType.Negative, NewLine);
					}
					else
					{
						input = new Input(ch);
					}
					NFA.AddTransition(transitionMap, start, input, new int[] { end });
				}

				return new StartAndEnd(start, end);
			}
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
