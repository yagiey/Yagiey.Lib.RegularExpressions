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
		public const char Plus = '+';
		public const char Minus = '-';


		public const char Tab = '\t';
		public const char CarriageReturn = '\r';
		public const char LineFeed = '\n';
		public const char Space = ' ';
		public const char Underline = '_';

		public const char EscapedTab = 't';
		public const char EscapedCr = 'r';
		public const char EscapedLf = 'n';
		public const char EscapedDigit = 'd';
		public const char EscapedWhiteSpace = 's';
		public const char EscapedIdentifier = 'w';

		public static IEnumerable<char> EscapedChar
		{
			get
			{
				return Enumerable.Empty<char>()
					.Append(EscapedTab)
					.Append(EscapedCr)
					.Append(EscapedLf)
					.Append(EscapedDigit)
					.Append(EscapedWhiteSpace)
					.Append(EscapedIdentifier)
					;
			}
		}
	}
}
