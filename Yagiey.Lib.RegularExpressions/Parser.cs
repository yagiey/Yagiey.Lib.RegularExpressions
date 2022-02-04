﻿using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Expressions;

namespace Yagiey.Lib.RegularExpressions
{
	using NFATransitionMap = IDictionary<TransitionParameters, IEnumerable<int>>;
	using ParserReturnValue = Tuple<int, int, IExpression>;

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
	/// spaceial_characters = backslash | vertical_bar | asterisk | lparen | rparen | question
	/// 
	/// backslash = '\'
	/// vertical_bar = '|'
	/// asterisk = '*'
	/// lparen = '('
	/// rparen = ')'
	/// question = '?'
	/// </definition>
	internal class Parser
	{
		public NondeterministicFiniteAutomaton EpsilonNFA
		{
			get;
			private set;
		}

		public IExpression Expression
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

			EpsilonNFA = new NondeterministicFiniteAutomaton(startNode, acceptingNodeSet, transitionMap);
			Expression = ret.Item1.Item3;
		}

		#region parsing

		private static Tuple<ParserReturnValue, NFATransitionMap> GetExpression(IEnumerator<Tuple<Token, Token?[]>> itorToken)
		{
			NFATransitionMap transitionMap = new Dictionary<TransitionParameters, IEnumerable<int>>();
			var itorID = new SequenceNumberEnumerator(0);
			ParserReturnValue result = GetExpression(transitionMap, itorToken, itorID);
			return Tuple.Create(result, transitionMap);
		}

		private static ParserReturnValue GetExpression(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv2 = GetLevel2(transitionMap, itorToken, itorID);
			int start1st = lv2.Item1;
			int end1st = lv2.Item2;
			IExpression expr = lv2.Item3;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && next.TokenType == TokenType.Selection)
			{
				List<IExpression> list = new();
				List<Tuple<int, Input, int[]>> t = new();

				list.Add(expr);
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
					IExpression exprNew = lv2New.Item3;
					list.Add(exprNew);
					t.Add(Tuple.Create(s, Input.Empty, new int[] { start2nd }));
					t.Add(Tuple.Create(end2nd, Input.Empty, new int[] { e }));

					next = itorToken.Current.Item2[0];
					if (next == null || next.TokenType != TokenType.Selection)
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
					AddTransition(transitionMap, new TransitionParameters(item.Item1, item.Item2), item.Item3);
				}
				expr = new Selection(list, s, e);
				start1st = s;
				end1st = e;
			}

			return new ParserReturnValue(start1st, end1st, expr);
		}

		private static ParserReturnValue GetLevel2(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv1 = GetLevel1(transitionMap, itorToken, itorID);
			int start1st = lv1.Item1;
			int end1st = lv1.Item2;
			IExpression expr = lv1.Item3;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && (next.TokenType == TokenType.Character || next.TokenType == TokenType.LParen))
			{
				List<IExpression> list = new();
				List<Tuple<int, Input, int[]>> t = new();

				list.Add(expr);
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
					IExpression expr2 = lv1New.Item3;

					list.Add(expr2);
					t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { start2nd }));
					prevEnd = end2nd;

					next = itorToken.Current.Item2[0];
				} while (next != null && (next.TokenType == TokenType.Character || next.TokenType == TokenType.LParen));

				t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { e }));

				foreach (var item in t)
				{
					AddTransition(transitionMap, new TransitionParameters(item.Item1, item.Item2), item.Item3);
				}
				expr = new Concatenation(list, s, e);
				start1st = s;
				end1st = e;
			}

			return new ParserReturnValue(start1st, end1st, expr);
		}

		private static ParserReturnValue GetLevel1(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv0 = GetLevel0(transitionMap, itorToken, itorID);
			int inStart = lv0.Item1;
			int inEnd = lv0.Item2;
			IExpression expr = lv0.Item3;

			Token? next = itorToken.Current.Item2[0];
			if (next != null && (next.TokenType == TokenType.Repeat0 || next.TokenType == TokenType.Option))
			{
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				int start;
				int end;
				if (next.TokenType == TokenType.Repeat0)
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					int node1 = itorID.Current;
					itorID.MoveNext();
					int node2 = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					expr = new Repeat0(expr, start, end);

					var t1 = Tuple.Create(start, Input.Empty, new int[] { node1, end });
					var t2 = Tuple.Create(node1, Input.Empty, new int[] { inStart });
					var t3 = Tuple.Create(inEnd, Input.Empty, new int[] { node2 });
					var t4 = Tuple.Create(node2, Input.Empty, new int[] { node1, end });

					var p1 = new TransitionParameters(t1.Item1, t1.Item2);
					AddTransition(transitionMap, p1, t1.Item3);
					var p2 = new TransitionParameters(t2.Item1, t2.Item2);
					AddTransition(transitionMap, p2, t2.Item3);
					var p3 = new TransitionParameters(t3.Item1, t3.Item2);
					AddTransition(transitionMap, p3, t3.Item3);
					var p4 = new TransitionParameters(t4.Item1, t4.Item2);
					AddTransition(transitionMap, p4, t4.Item3);
				}
				else
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					expr = new Option(expr, start, end);

					var t1 = Tuple.Create(start, Input.Empty, new int[] { inStart });
					var t2 = Tuple.Create(inEnd, Input.Empty, new int[] { end });
					var t3 = Tuple.Create(start, Input.Empty, new int[] { end });

					var p1 = new TransitionParameters(t1.Item1, t1.Item2);
					AddTransition(transitionMap, p1, t1.Item3);
					var p2 = new TransitionParameters(t2.Item1, t2.Item2);
					AddTransition(transitionMap, p2, t2.Item3);
					var p3 = new TransitionParameters(t3.Item1, t3.Item2);
					AddTransition(transitionMap, p3, t3.Item3);
				}

				return new ParserReturnValue(start, end, expr);
			}
			else
			{
				return new ParserReturnValue(inStart, inEnd, expr);
			}
		}

		private static ParserReturnValue GetLevel0(NFATransitionMap transitionMap, IEnumerator<Tuple<Token, Token?[]>> itorToken, IEnumerator<int> itorID)
		{
			if (!itorToken.MoveNext() || itorToken.Current == null)
			{
				throw new Exception();
			}

			Token curr = itorToken.Current.Item1;
			if (curr.TokenType == TokenType.Character)
			{
				char ch = curr.Source[0];

				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;

				IExpression expr = new Expressions.Character(ch, start, end);

				TransitionParameters p = new(start, new Input(ch));
				int[] ends = new int[] { end };
				AddTransition(transitionMap, p, ends);

				return new ParserReturnValue(start, end, expr);
			}
			else
			{
				if (curr.TokenType != TokenType.LParen)
				{
					throw new Exception();
				}

				var ret = GetExpression(transitionMap, itorToken, itorID);

				Token? next = itorToken.Current.Item2[0];
				if (next == null || next.TokenType != TokenType.RParen)
				{
					throw new Exception();
				}
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				return ret;
			}
		}

		private static NFATransitionMap AddTransition(NFATransitionMap transitionMap, TransitionParameters p, IEnumerable<int> dest)
		{
			if (transitionMap.ContainsKey(p))
			{
				var value = transitionMap[p];
				transitionMap[p] = value.Concat(dest);
			}
			else
			{
				transitionMap.Add(p, dest);
			}
			return transitionMap;
		}

		#endregion
	}
}