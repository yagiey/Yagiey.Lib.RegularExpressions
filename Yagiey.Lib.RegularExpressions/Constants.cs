using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class Constants
	{
		public const char Backslash = '\\';
		public const char Dot = '.';

		public const char LParen = '(';
		public const char RParen = ')';
		public const char VerticalBar = '|';
		public const char Asterisk = '*';
		public const char Question = '?';
		public const char Hat = '^';
		public const char LBracket = '[';
		public const char RBracket = ']';
		public const char LBrace = '{';
		public const char RBrace = '}';

		public static IEnumerable<char> Operators
		{
			get
			{
				return Enumerable.Empty<char>()
					.Append(VerticalBar)
					.Append(Asterisk)
					.Append(Question)
					.Append(Hat)

					.Append(LParen)
					.Append(RParen)

					.Append(LBracket)
					.Append(RBracket)

					.Append(LBrace)
					.Append(RBrace)
					;
			}
		}
	}
}
