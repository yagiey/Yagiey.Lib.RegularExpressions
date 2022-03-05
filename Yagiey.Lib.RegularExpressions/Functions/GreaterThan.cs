using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class GreaterThan<T> : IRange<T>, IComparable<GreaterThan<T>>, IEquatable<GreaterThan<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		public GreaterThan(T x)
		{
			Value = x;
		}

		public T Value
		{
			get;
			private set;
		}

		public T? LowerLimit
		{
			get
			{
				return Value;
			}
		}

		public T? UpperLimit
		{
			get
			{
				return default;
			}
		}

		public int CompareTo(GreaterThan<T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			return LowerLimit!.CompareTo(other.LowerLimit!);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is GreaterThan<T>)
			{
				GreaterThan<T>? o = other as GreaterThan<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(GreaterThan<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return LowerLimit!.Equals(other.LowerLimit!);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			GreaterThan<T>? o = other as GreaterThan<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			GreaterThan<T>? o = other as GreaterThan<T>;
			return Equals(o);
		}

		public static bool operator ==(GreaterThan<T>? lhs, GreaterThan<T>? rhs)
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

		public static bool operator !=(GreaterThan<T>? lhs, GreaterThan<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			return LowerLimit!.GetHashCode();
		}

		public bool Invoke(T x)
		{
			return Value.CompareTo(x) < 0;
		}

		public bool IsLowerBounded()
		{
			return true;
		}

		public bool IsUpperBounded()
		{
			return false;
		}

		public override string ToString()
		{
			return "(GreaterThan " + Value + ")";
		}
	}
}
