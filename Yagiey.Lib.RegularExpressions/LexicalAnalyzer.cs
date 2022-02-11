using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Extensions.Generic;

namespace Yagiey.Lib.RegularExpressions
{
	internal class LexicalAnalyzer : IEnumerable<Tuple<Token, Token?[]>>
	{
		private readonly IDeterministicFiniteAutomaton<char> _special;
		private readonly IDeterministicFiniteAutomaton<char> _escape;
		private readonly IDeterministicFiniteAutomaton<char> _chara;
		private IEnumerable<IDeterministicFiniteAutomaton<char>> _automata;

		public string Source
		{
			get;
			private set;
		}

		public LexicalAnalyzer(string source)
		{
			_special = new SpecialCharacter();
			_escape = new Escape();
			_chara = new Character();
			_automata = Enumerable.Empty<IDeterministicFiniteAutomaton<char>>();

			Source = source;
			SetupAutomata();
		}

		#region automata

		private void SetupAutomata()
		{
			_automata =
				Enumerable.Empty<IDeterministicFiniteAutomaton<char>>()
				.Append(_special)
				.Append(_escape)
				.Append(_chara);
			_automata.ForEach(a => a.Reset());
		}

		private void MoveNextAutomata(char ch)
		{
			_automata.ForEach(a => a.MoveNext(ch));
			_automata = _automata.Where(a => !a.IsError());
		}

		private int CountNextAlive(char ch)
		{
			return _automata.Count(a => !a.IsNextError(ch));
		}

		#endregion

		private Token? GetToken(string str)
		{
			var a = _automata.Where(a => a.IsAcceptable()).FirstOrDefault();
			if (a == null)
			{
				return null;
			}
			return GetToken(a, str);
		}

		private Token GetToken(IDeterministicFiniteAutomaton<char> a, string strToken)
		{
			TokenType type;
			if (a == _special)
			{
				char ch = strToken[0];
				if (ch == Constants.LParen)
				{
					type = TokenType.LParen;
				}
				else if (ch == Constants.RParen)
				{
					type = TokenType.RParen;
				}
				else if (ch == Constants.VerticalBar)
				{
					type = TokenType.Selection;
				}
				else if (ch == Constants.Question)
				{
					type = TokenType.Option;
				}
				else if (ch == Constants.Dot)
				{
					type = TokenType.AnyButReturn;
				}
				else
				{
					type = TokenType.Repeat0;
				}
			}
			else if (a == _escape)
			{
				strToken = strToken.TrimStart('\\');
				type = TokenType.Character;
			}
			else
			{
				type = TokenType.Character;
			}

			return new Token(strToken, type);
		}

		public IEnumerable<Token> EnumerateTokens()
		{
			int position = -1;
			StringBuilder sb = new();
			Func<IEnumerable<char>, int, IEnumerable<Tuple<char, char?[]>>> EnumerateLookingAhead =
			Extensions.Generic.ValueType.EnumerableExtension.EnumerateLookingAhead;
			foreach (var item in EnumerateLookingAhead(Source, 1))
			{
				position++;
				char current = item.Item1;
				char? next = item.Item2[0];

				MoveNextAutomata(current);
				sb.Append(current);

				int c1al = _automata.Count();
				int c1ac = _automata.Count(a => a.IsAcceptable());

				if (c1al == 0)
				{
					string errorMsg = string.Format("invalid token '{0}' detected", sb.ToString());
					throw new Exception(errorMsg);
				}

				if (next.HasValue)
				{
					int c2al = CountNextAlive(next.Value);
					if (c2al == 0)
					{
						if (c1ac > 0)
						{
							var token1 = GetToken(sb.ToString());
							yield return token1!;

							SetupAutomata();
							sb.Clear();
						}
						else
						{
							// just before error
						}
					}
				}
			}

			var token2 = GetToken(sb.ToString());
			if (token2 == null)
			{
				string errorMsg = string.Format("invalid token '{0}' detected", sb.ToString());
				throw new Exception(errorMsg);
			}
			yield return token2;
		}

		#region IEnumerable

		public IEnumerator<Tuple<Token, Token?[]>> GetEnumerator()
		{
			Func<IEnumerable<Token>, int, IEnumerable<Tuple<Token, Token?[]>>> func = Extensions.Generic.ReferenceType.EnumerableExtension.EnumerateLookingAhead;
			return func(EnumerateTokens(), 1).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
