using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	public interface IUnaryFunctionObject<TRet, TArg> : IEquatable<IUnaryFunctionObject<TRet, TArg>>, IComparable<IUnaryFunctionObject<TRet, TArg>>
	{
		TRet Invoke(TArg arg);

		string ToRegularExpression();
	}
}
