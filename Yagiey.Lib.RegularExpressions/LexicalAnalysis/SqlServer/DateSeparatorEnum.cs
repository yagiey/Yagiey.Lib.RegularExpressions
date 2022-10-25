using System;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.SqlServer
{
	[Flags]
	public enum DateSeparatorEnum
	{
		None = 0,
		Slash = 1,
		Dot = 2,
		Dash = 4,
	}
}
