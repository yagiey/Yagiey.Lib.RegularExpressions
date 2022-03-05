using System;
using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class GELE<T> : IRange<T>, IComparable<GELE<T>>, IEquatable<GELE<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly Tuple<GreaterThanOrEqual<T>, LessThanOrEqual<T>> _and;

		public GELE(T lowerLimit, T upperLimit)
		{
			if (lowerLimit.CompareTo(upperLimit) > 0)
			{
				T tmp = lowerLimit;
				lowerLimit = upperLimit;
				upperLimit = tmp;
			}
			_and = Tuple.Create(new GreaterThanOrEqual<T>(lowerLimit), new LessThanOrEqual<T>(upperLimit));
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
			return _and.Item1.Invoke(x) && _and.Item2.Invoke(x);
		}

		public T? LowerLimit
		{
			get
			{
				return _and.Item1.Value;
			}
		}

		public T? UpperLimit
		{
			get
			{
				return _and.Item2.Value;
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
			return "(GELE " + _and.Item1.Value + " " + _and.Item2.Value + ")";
		}
	}
}
