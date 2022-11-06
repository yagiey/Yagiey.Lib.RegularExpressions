using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal interface IRange<T> : IUnaryFunctionObject<bool, T> where T : IComparable<T>
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
