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
			else if (x is GreaterThan<TArg>)
			{
				return 2;
			}
			else if (x is GreaterThanOrEqual<TArg>)
			{
				return 3;
			}
			else if (x is LessThan<TArg>)
			{
				return 4;
			}
			else if (x is LessThanOrEqual<TArg>)
			{
				return 5;
			}
			else if (x is GTLT<TArg>)
			{
				return 6;
			}
			else if (x is GTLE<TArg>)
			{
				return 7;
			}
			else if (x is GELT<TArg>)
			{
				return 8;
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
			else if (x is GreaterThan<TArg>)
			{
				GreaterThan<TArg> x_ = (GreaterThan<TArg>)x;
				GreaterThan<TArg> y_ = (GreaterThan<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is GreaterThanOrEqual<TArg>)
			{
				GreaterThanOrEqual<TArg> x_ = (GreaterThanOrEqual<TArg>)x;
				GreaterThanOrEqual<TArg> y_ = (GreaterThanOrEqual<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is LessThan<TArg>)
			{
				LessThan<TArg> x_ = (LessThan<TArg>)x;
				LessThan<TArg> y_ = (LessThan<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is LessThanOrEqual<TArg>)
			{
				LessThanOrEqual<TArg> x_ = (LessThanOrEqual<TArg>)x;
				LessThanOrEqual<TArg> y_ = (LessThanOrEqual<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is GTLT<TArg>)
			{
				GTLT<TArg> x_ = (GTLT<TArg>)x;
				GTLT<TArg> y_ = (GTLT<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is GTLE<TArg>)
			{
				GTLE<TArg> x_ = (GTLE<TArg>)x;
				GTLE<TArg> y_ = (GTLE<TArg>)y;
				return x_.CompareTo(y_);
			}
			else if (x is GELT<TArg>)
			{
				GELT<TArg> x_ = (GELT<TArg>)x;
				GELT<TArg> y_ = (GELT<TArg>)y;
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
