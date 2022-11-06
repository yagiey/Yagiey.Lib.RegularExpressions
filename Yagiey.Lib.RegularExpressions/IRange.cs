using System;

namespace Yagiey.Lib.RegularExpressions
{
	internal interface IRange<T> where T : IComparable<T>
	{
		bool IsLowerBounded();

		bool IsUpperBounded();

		T? LowerLimit
		{
			get;
		}

		T? UpperLimit
		{
			get;
		}
	}
}
