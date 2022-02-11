using System;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	internal class Character : IExpression
	{
		public Input Input
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

		public Character(Input input, int start, int end)
		{
			Input = input;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return Input.ToString();
		}
	}
}
