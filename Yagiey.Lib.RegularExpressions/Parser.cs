using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Expressions;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions
{
	using NFA = NondeterministicFiniteAutomaton;
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

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
			IEnumerator<int> itorID = new SequenceNumberEnumerator(0);

			IExpression result = GetExpression(itorToken, itorID);

			NFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, IEnumerable<int>>>();
			result.AddTransition(transitionMap, itorID);

			int startNode = result.Start;
			IEnumerable<int> acceptingNodeSet = new int[] { result.End };

			EpsilonNFA = new NFA(startNode, acceptingNodeSet, transitionMap);
		}

		private IEnumerator<Tuple<char, char?[]>> GetEnumerator()
		{
			Func<IEnumerable<char>, int, IEnumerable<Tuple<char, char?[]>>> func = Extensions.Generic.ValueType.EnumerableExtension.EnumerateLookingAhead;
			return func(Source, 2).GetEnumerator();
		}

		#region parsing

		private static IExpression GetExpression(IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv2 = GetLevel2(itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfSelection())
			{

				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				List<IExpression> subs = new() { lv2 };
				do
				{
					var lv2New = GetLevel2(itorToken, itorID);
					subs.Add(lv2New);
					if (!itorToken.Current.CanNextBeRegardedAsRestOfSelection())
					{
						break;
					}
					if (!itorToken.MoveNext())
					{
						throw new Exception();
					}
				} while (true);
				return new Selection(subs, itorID);
			}
			else
			{
				return lv2;
			}
		}

		private static IExpression GetLevel2(IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv1 = GetLevel1(itorToken, itorID);

			if (itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
			{
				List<IExpression> subs = new() { lv1 };
				do
				{
					var lv1New = GetLevel1(itorToken, itorID);
					subs.Add(lv1New);
					if (!itorToken.Current.CanNextBeRegardedAsRestOfConcatenation())
					{
						break;
					}
				} while (true);
				return new Concatenation(subs, itorID);
			}
			else
			{
				return lv1;
			}
		}

		private static IExpression GetLevel1(IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			var lv0 = GetLevel0(itorToken, itorID);

			char? next = itorToken.Current.Item2[0];
			if (next.HasValue && (next == Constants.Asterisk || next == Constants.Plus || next == Constants.Question))
			{
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				if (next == Constants.Question)
				{
					return new Option(lv0, itorID);
				}
				else if (next == Constants.Asterisk)
				{
					return new Repeat0(lv0, itorID);
				}
				else// if (next == Constants.Plus)
				{
					return new Repeat1(lv0, itorID);
				}
			}
			else
			{
				return lv0;
			}
		}

		private static IExpression GetLevel0(IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
			if (!itorToken.MoveNext())
			{
				throw new Exception();
			}

			if (itorToken.Current.IsBeginingOfExpressionWithParen())
			{
				var expr = GetExpression(itorToken, itorID);

				if (!itorToken.Current.IsNextEndOfExpressionWithParen())
				{
					throw new Exception();
				}
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				return expr;
			}
			else if (itorToken.Current.IsBeginingOfCharacterClassWithBracket())
			{
				var charClass = GetCharacterClass(itorToken, itorID);

				if (!itorToken.Current.IsNextEndOfCharacterClassWithBracket())
				{
					throw new Exception();
				}
				if (!itorToken.MoveNext())
				{
					throw new Exception();
				}

				return charClass;
			}
			else
			{
				char current = itorToken.Current.Item1;
				char? next = itorToken.Current.Item2[0];

				if (current == Constants.Backslash && next.HasValue && Constants.EscapedCharacters.Any(_ => _ == next.Value))
				{
					itorToken.MoveNext();

					char nextChar = next.Value;
					IList<char> characters;
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
						characters = Enumerable.Range(0, 10).Select(n => Convert.ToChar('0' + n)).ToList();
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
							.Append(Constants.Underline)
							.ToList();
					}

					if (characters.Count == 1)
					{
						return new Character(new Input(characters[0]), itorID);
					}
					else
					{
						itorID.MoveNext();
						int startInner = itorID.Current;
						itorID.MoveNext();
						int endInner = itorID.Current;

						return
							new Selection(
								characters.Select(it => new Character(new Input(it), startInner, endInner)),
								itorID
							);
					}
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
					return new Character(input, itorID);
				}
			}
		}

		private static IExpression GetCharacterClass(IEnumerator<Tuple<char, char?[]>> itorToken, IEnumerator<int> itorID)
		{
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
			return new Character(input, itorID);
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

		#endregion
	}
}
