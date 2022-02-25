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
			if (a == _escape)
			{
				strToken = strToken.TrimStart('\\');
				type = TokenType.Escape;
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
							string strToken = sb.ToString();
							var token1 = GetToken(strToken);
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

			string strToken2 = sb.ToString();
			var token2 = GetToken(strToken2);
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
