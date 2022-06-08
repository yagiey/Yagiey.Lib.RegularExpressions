using System;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class UnaryFunctionObjectComparer<TRet, TArg> : IComparer<IUnaryFunctionObject<TRet, TArg>>
		where TArg : IComparable<TArg>, IEquatable<TArg>
	{
		public static int GetClassID(IUnaryFunctionObject<TRet, TArg>? x)
		{
			if (x == null)
			{
				return 0;
			}
			else if (x is Equal<TArg>)
			{
				return 1;
			}
			else if (x is GELE<TArg>)
			{
				return 9;
			}
			else if (x is And<TArg>)
			{
				return 10;
			}
			else if (x is Or<TArg>)
			{
				return 11;
			}
			else if (x is Not<TArg>)
			{
				return 12;
			}
			else
			{
				throw new Exception();
			}
		}

		public int Compare(IUnaryFunctionObject<TRet, TArg>? x, IUnaryFunctionObject<TRet, TArg>? y)
		{
			if ((x is null && y is null) || ReferenceEquals(x, y))
			{
				return 0;
			}
			else if (x is null)
			{
				return -1;
			}
			else if (y is null)
			{
				return 1;
			}
			else if (x.GetType() != y.GetType())
			{
				int _x = GetClassID(x);
				int _y = GetClassID(y);
				return _x.CompareTo(_y);
			}
			else if (x is Equal<TArg>)
			{
				Equal<TArg> x_ = (Equal<TArg>)x;
				Equal<TArg> y_ = (Equal<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is GELE<TArg>)
			{
				GELE<TArg> x_ = (GELE<TArg>)x;
				GELE<TArg> y_ = (GELE<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is And<TArg>)
			{
				And<TArg> x_ = (And<TArg>)x;
				And<TArg> y_ = (And<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is Or<TArg>)
			{
				Or<TArg> x_ = (Or<TArg>)x;
				Or<TArg> y_ = (Or<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is Not<TArg>)
			{
#pragma warning disable IDE0020 // Use pattern matching
				Not<TArg> x_ = (Not<TArg>)x;
#pragma warning restore IDE0020 // Use pattern matching
				Not<TArg> y_ = (Not<TArg>)y;
				return x_.CompareTo(y_);
			}
			else
			{
				throw new Exception();
			}
		}
	}
}
