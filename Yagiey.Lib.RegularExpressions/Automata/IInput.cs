using System;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal interface IInput : IComparable<IInput>
	{
		bool IsEmpty { get; }

		bool IsLiteral { get; }

		bool IsPredicate { get; }

		bool Match(char ch);

		string ToRegularExpression();
	}
}
