using System;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	internal class Character : IExpression
	{
		public char Char
		{
			get;
			private set;
		}

		public int Start
		{
			get;
			private set;
		}

		public int End
		{
			get;
			private set;
		}

		public Character(char ch, int start, int end)
		{
			Char = ch;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return Char.ToString();
		}
	}
}
