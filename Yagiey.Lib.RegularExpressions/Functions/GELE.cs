using System;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class GELE<T> : IRange<T>, IComparable<GELE<T>>, IEquatable<GELE<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly T _lowerLimit;
		private readonly T _upperLimit;

		public GELE(T lowerLimit, T upperLimit)
		{
			if (lowerLimit.CompareTo(upperLimit) > 0)
			{
				(upperLimit, lowerLimit) = (lowerLimit, upperLimit);
			}
			_lowerLimit = lowerLimit;
			_upperLimit = upperLimit;
		}

		public int CompareTo(GELE<T>? other)
		{
			if (other is null)
			{
				return 1;
			}

			IEnumerable<T> x = new T[] { LowerLimit!, UpperLimit! };
			IEnumerable<T> y = new T[] { other.LowerLimit!, other.UpperLimit! };
			return new EnumerableComparer<T>().Compare(x, y);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is GELE<T>)
			{
				GELE<T>? o = other as GELE<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(GELE<T>? other)
		{
			if (other is null)
			{
				return false;
			}

			IEnumerable<T> x = new T[] { LowerLimit!, UpperLimit! };
			IEnumerable<T> y = new T[] { other.LowerLimit!, other.UpperLimit! };
			return new EnumerableEqualityComparer<T>().Equals(x, y);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			GELE<T>? o = other as GELE<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			GELE<T>? o = other as GELE<T>;
			return Equals(o);
		}

		public static bool operator ==(GELE<T>? lhs, GELE<T>? rhs)
		{
			if (lhs is null && rhs is null)
			{
				return true;
			}
			else if (lhs is null)
			{
				return false;
			}
			else if (rhs is null)
			{
				return false;
			}
			else
			{
				return lhs.Equals(rhs);
			}
		}

		public static bool operator !=(GELE<T>? lhs, GELE<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			string strObj = string.Format("{0}{1}", LowerLimit!, UpperLimit!);
			return strObj.GetHashCode();
		}

		public bool Invoke(T x)
		{
			return
				(_lowerLimit.Equals(x) || _lowerLimit.CompareTo(x) <= 0)
				&& (x.Equals(_upperLimit) || x.CompareTo(_upperLimit) <= 0);
		}

		public T? LowerLimit
		{
			get
			{
				return _lowerLimit;
			}
		}

		public T? UpperLimit
		{
			get
			{
				return _upperLimit;
			}
		}

		public bool IsLowerBounded()
		{
			return true;
		}

		public bool IsUpperBounded()
		{
			return true;
		}

		public override string ToString()
		{
			return "(GELE " + _lowerLimit + " " + _upperLimit + ")";
		}

		public string ToRegularExpression()
		{
			return string.Format("{0}-{1}", _lowerLimit, _upperLimit);
		}
	}
}
