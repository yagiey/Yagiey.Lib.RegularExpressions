using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class Constants
	{
		public const char Backslash = '\\';

		public const char LParen = '(';
		public const char RParen = ')';
		public const char VerticalBar = '|';
		public const char Asterisk = '*';
		public const char Question = '?';

		public static IEnumerable<char> SpecialCharacters
		{
			get
			{
				return
					Enumerable.Empty<char>()
					.Append(LParen)
					.Append(RParen)
					.Append(VerticalBar)
					.Append(Asterisk)
					.Append(Question)
					.ToArray();
			}
		}
	}
}
