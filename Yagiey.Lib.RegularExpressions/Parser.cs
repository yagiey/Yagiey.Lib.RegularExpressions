using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;
	using SubNFA = Tuple<int, int, IDictionary<int, IDictionary<IInput, IEnumerable<int>>>>;

	/// <summary>
	/// parser for regular expression
	/// </summary>
	/// <definition>
	/// expression = level2, { vertical_bar, level2 }
	/// level2 = level1, { level1 }
	/// level1 = level0 | level0, question | level0, asterisk | level0, plus
	/// level0 = all_characters | lparen, expression, rparen
	/// 
	/// all_characters = __all_characters__except(spaceial_characters) | escape
	/// escape = backslash, spaceial_characters
	/// spaceial_characters = backslash | vertical_bar | asterisk | lparen | rparen | question | dot
	/// 
	/// backslash = '\'
	/// vertical_bar = '|'
	/// asterisk = '*'
	/// plus = '+'
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

		public string Source
		{
			get;
			private set;
		}

		public Parser(string source)
		{
			Source = source;
			var itorToken = GetEnumerator();
			NFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
			IEnumerator<int> itorID = new SequenceNumberEnumerator(0);

			SubNFA result = GetExpression(transitionMap, itorToken, itorID);

			int startNode = result.Item1;
			IEnumerable<int> acceptingNodeSet = new int[] { result.Item2 };

			EpsilonNFA = new NFA(startNode, acceptingNodeSet, transitionMap);
		}

		private IEnumerator<Tuple<char, char?[]>> GetEnumerator()
		{
			Func<IEnumerable<char>, int, IEnumerable<Tuple<char, char?[]>>> func = Extensions.Generic.ValueType.EnumerableExtension.EnumerateLookingAhead;
			return func(Source, 2).GetEnumerator();
		}

		#region parsing

		private static SubNFA GetExpression(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv2 = GetLevel2(transitionMap, itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfSelection())
			{
				List<Tuple<int, Input, int[]>> t = new();

				NFATransitionMap thisNFATrans = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
				List<NFATransitionMap> subs = new()
				{
					lv2.Item3
				};

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
					subs.Add(lv2New.Item3);

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

				foreach (var sub in subs)
				{
					MergeSubNFATransition(sub, thisNFATrans);
				}

				foreach (var item in t)
				{
					NFA.AddTransition(thisNFATrans, item.Item1, item.Item2, item.Item3);
					NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
				}
				return new SubNFA(start, end, thisNFATrans);
			}
			else
			{
				return lv2;
			}
		}

		private static SubNFA GetLevel2(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv1 = GetLevel1(transitionMap, itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
			{
				List<Tuple<int, Input, int[]>> t = new();
				NFATransitionMap thisNFATrans = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
				List<NFATransitionMap> subs = new()
				{
					lv1.Item3
				};

				itorID.MoveNext();
				int start = itorID.Current;
				itorID.MoveNext();
				int end = itorID.Current;
				t.Add(Tuple.Create(start, Input.Empty, new int[] { lv1.Item1 }));

				int prevEnd = lv1.Item2;
				do
				{
					var lv1New = GetLevel1(transitionMap, itorToken, itorID);
					subs.Add(lv1New.Item3);

					t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { lv1New.Item1 }));
					prevEnd = lv1New.Item2;

					if (!itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
					{
						break;
					}
				} while (true);

				t.Add(Tuple.Create(prevEnd, Input.Empty, new int[] { end }));

				foreach (var sub in subs)
				{
					MergeSubNFATransition(sub, thisNFATrans);
				}

				foreach (var item in t)
				{
					NFA.AddTransition(thisNFATrans, item.Item1, item.Item2, item.Item3);
					NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
				}
				return new SubNFA(start, end, thisNFATrans);
			}
			else
			{
				return lv1;
			}
		}

		private static SubNFA GetLevel1(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv0 = GetLevel0(transitionMap, itorToken, itorID);

			char? next = itorToken.Current.Item2[0];
			if (next.HasValue && (next == Constants.Asterisk || next == Constants.Plus || next == Constants.Question))
			{
				NFATransitionMap thisNFATrans = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();

				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				int start;
				int end;
				if (next == Constants.Question)
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					List<Tuple<int, Input, int[]>> t = new() {
						Tuple.Create(start, Input.Empty, new int[] { lv0.Item1 }),
						Tuple.Create(lv0.Item2, Input.Empty, new int[] { end }),
						Tuple.Create(start, Input.Empty, new int[] { end }),
					};

					MergeSubNFATransition(lv0.Item3, thisNFATrans);
					foreach (var item in t)
					{
						NFA.AddTransition(thisNFATrans, item.Item1, item.Item2, item.Item3);
						NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
					}
				}
				else if (next == Constants.Asterisk)
				{
					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					int node1 = itorID.Current;
					itorID.MoveNext();
					int node2 = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					List<Tuple<int, Input, int[]>> t = new() {
						Tuple.Create(start, Input.Empty, new int[] { node1, end }),
						Tuple.Create(node1, Input.Empty, new int[] { lv0.Item1 }),
						Tuple.Create(lv0.Item2, Input.Empty, new int[] { node2 }),
						Tuple.Create(node2, Input.Empty, new int[] { node1, end }),
					};

					MergeSubNFATransition(lv0.Item3, thisNFATrans);
					foreach (var item in t)
					{
						NFA.AddTransition(thisNFATrans, item.Item1, item.Item2, item.Item3);
						NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
					}
				}
				else// if (next == Constants.Plus)
				{
					var sub = RenumberSubNFATransition(lv0, itorID);

					itorID.MoveNext();
					start = itorID.Current;
					itorID.MoveNext();
					int node1 = itorID.Current;
					itorID.MoveNext();
					int node2 = itorID.Current;
					itorID.MoveNext();
					int node3 = itorID.Current;
					itorID.MoveNext();
					end = itorID.Current;

					List<Tuple<int, Input, int[]>> t = new() {
						Tuple.Create(start, Input.Empty, new int[] { lv0.Item1 }),
						Tuple.Create(lv0.Item2, Input.Empty, new int[] { node1 }),
						Tuple.Create(node1, Input.Empty, new int[] { node2 }),
						Tuple.Create(node1, Input.Empty, new int[] { end }),
						Tuple.Create(node2, Input.Empty, new int[] { sub.Item1 }),
						Tuple.Create(sub.Item2, Input.Empty, new int[] { node3 }),
						Tuple.Create(node3, Input.Empty, new int[] { node2 }),
						Tuple.Create(node3, Input.Empty, new int[] { end }),
					};

					MergeSubNFATransition(lv0.Item3, thisNFATrans);
					MergeSubNFATransition(sub.Item3, thisNFATrans);
					MergeSubNFATransition(sub.Item3, transitionMap);
					foreach (var item in t)
					{
						NFA.AddTransition(thisNFATrans, item.Item1, item.Item2, item.Item3);
						NFA.AddTransition(transitionMap, item.Item1, item.Item2, item.Item3);
					}
				}

				return new SubNFA(start, end, thisNFATrans);
			}
			else
			{
				return lv0;
			}
		}

		private static SubNFA GetLevel0(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
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

				NFATransitionMap thisNFATrans = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();

				if (current == Constants.Backslash && next.HasValue && Constants.EscapedCharacters.Any(_ => _ == next.Value))
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
					else// if (nextChar == Constants.EscapedIdentifier)
					{
						characters = Enumerable.Empty<char>()
							.Concat(Enumerable.Range(0, 26).Select(n => Convert.ToChar('a' + n)))
							.Concat(Enumerable.Range(0, 26).Select(n => Convert.ToChar('A' + n)))
							.Concat(Enumerable.Range(0, 10).Select(n => Convert.ToChar('0' + n)))
							.Append(Constants.Underline);
					}

					AddTransitions(transitionMap, start, characters, new int[] { end });
					AddTransitions(thisNFATrans, start, characters, new int[] { end });
				}
				else
				{
					IInput input;
					if (current == Constants.Dot)
					{
						Not<char> pred = new(new Equal<char>(Constants.LineFeed));
						input = new InputWithPredicate(pred);
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
					NFA.AddTransition(thisNFATrans, start, input, new int[] { end });
				}

				return new SubNFA(start, end, thisNFATrans);
			}
		}

		private static SubNFA GetCharacterClass(NFATransitionMap transitionMap, IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			NFATransitionMap thisNFATrans = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();

			itorID.MoveNext();
			int start = itorID.Current;
			itorID.MoveNext();
			int end = itorID.Current;

			char? next1 = itorToken.Current.Item2[0];
			bool isNegative = next1.HasValue && next1 == Constants.Hat;

			List<Tuple<char, char>> ranges = new();
			List<char> characters = new();

			bool isFirst = true;
			while (true)
			{
				if (isFirst && isNegative)
				{
					itorToken.MoveNext();
				}

				next1 = itorToken.Current.Item2[0];
				if (next1 == null)
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

				var current = GetNextCharacter(itorToken);
				char currentChar;
				if (current.Item1 && Constants.ControlCharacters.Any(it => it == current.Item2))
				{
					if (current.Item2 == Constants.EscapedTab)
					{
						currentChar = Constants.Tab;
					}
					else if (current.Item2 == Constants.EscapedCr)
					{
						currentChar = Constants.CarriageReturn;
					}
					else// if (current.Item2 == Constants.EscapedLf)
					{
						currentChar = Constants.LineFeed;
					}
				}
				else
				{
					currentChar = current.Item2;
				}

				next1 = itorToken.Current.Item2[0];
				char? next2 = itorToken.Current.Item2[1];

				bool isRange = next1.HasValue && next1.Value == Constants.Minus && next2.HasValue && next2.Value != Constants.RBracket;
				if (isRange)
				{
					char ch1 = currentChar;
					if (!itorToken.MoveNext() || !itorToken.Current.Item2[0].HasValue || itorToken.Current.Item2[0] == null)
					{
						const string errMsg = "insufficient character range";
						throw new Exception(errMsg);
					}

					current = GetNextCharacter(itorToken);
					currentChar = current.Item2;
					char ch2 = currentChar;

					ranges.Add(Tuple.Create(ch1, ch2));
				}
				else
				{
					characters.Add(currentChar);
				}

				isFirst = false;
			}

			IUnaryFunctionObject<bool, char> predicate;
			if (isNegative)
			{
				var predicates =
					Enumerable.Empty<IUnaryFunctionObject<bool, char>>()
						.Concat(characters.Select(c => new Not<char>(new Equal<char>(c))))
						.Concat(ranges.Select(r => new Not<char>(new GELE<char>(r.Item1, r.Item2))));
				if (1 == characters.Count + ranges.Count)
				{
					predicate = predicates.First();
				}
				else
				{
					predicate = new And<char>(predicates);
				}
			}
			else
			{
				var predicates =
					Enumerable.Empty<IUnaryFunctionObject<bool, char>>()
						.Concat(characters.Select(c => new Equal<char>(c)))
						.Concat(ranges.Select(r => new GELE<char>(r.Item1, r.Item2)));
				if (1 == characters.Count + ranges.Count)
				{
					predicate = predicates.First();
				}
				else
				{
					predicate = new Or<char>(predicates);
				}
			}

			IInput input = new InputWithPredicate(predicate);
			NFA.AddTransition(thisNFATrans, start, input, new int[] { end });
			NFA.AddTransition(transitionMap, start, input, new int[] { end });
			return new SubNFA(start, end, thisNFATrans);
		}

		private static Tuple<bool, char> GetNextCharacter(IEnumerator<Tuple<char, char?[]>> itorToken)
		{
			if (!itorToken.MoveNext())
			{
				const string ErrMsg = "unexpected EOF";
				throw new Exception(ErrMsg);
			}

			char crnt = itorToken.Current.Item1;
			bool isEscape = false;
			if (crnt == Constants.Backslash)
			{
				if (!itorToken.MoveNext())
				{
					const string ErrMsg = "insufficient escaped char";
					throw new Exception(ErrMsg);
				}
				isEscape = true;
				crnt = itorToken.Current.Item1;
			}
			return Tuple.Create(isEscape, crnt);
		}

		private static SubNFA RenumberSubNFATransition(SubNFA subNFA, IEnumerator<int> itorID)
		{
			HashSet<int> oldNodes = new();
			foreach (var pair1 in subNFA.Item3)
			{
				oldNodes.Add(pair1.Key);
				foreach (var pair2 in pair1.Value)
				{
					foreach (var dest in pair2.Value)
					{
						oldNodes.Add(dest);
					}
				}
			}

			IDictionary<int, int> mapOld2New = new Dictionary<int, int>();
			foreach (var oldNode in oldNodes)
			{
				itorID.MoveNext();
				int newNode = itorID.Current;
				mapOld2New.Add(oldNode, newNode);
			}

			NFATransitionMap result = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
			foreach (var pair1 in subNFA.Item3)
			{
				IDictionary<IInput, IEnumerable<int>> map2 = new Dictionary<IInput, IEnumerable<int>>();
				foreach (var pair2 in pair1.Value)
				{
					map2.Add(
						pair2.Key,
						pair2.Value.Select(it => mapOld2New[it]).OrderBy(it => it)
					);
				}
				result.Add(mapOld2New[pair1.Key], map2);
			}

			return new SubNFA(mapOld2New[subNFA.Item1], mapOld2New[subNFA.Item2], result);
		}

		private static void MergeSubNFATransition(NFATransitionMap mapFrom, NFATransitionMap mapDest)
		{
			foreach (var pair1 in mapFrom)
			{
				int from = pair1.Key;
				foreach (var pair2 in pair1.Value)
				{
					IInput input = pair2.Key;
					IEnumerable<int> dest = pair2.Value;
					NFA.AddTransition(mapDest, from, input, dest);
				}
			}
		}

		public static NFATransitionMap AddTransitions(NFATransitionMap transitionMap, int start, IEnumerable<char> characters, IEnumerable<int> ends)
		{
			foreach (char ch in characters)
			{
				Input input = new(ch);
				NFA.AddTransition(transitionMap, start, input, ends);
			}
			return transitionMap;
		}

		#endregion
	}
}
