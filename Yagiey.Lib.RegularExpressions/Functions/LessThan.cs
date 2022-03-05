using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class LessThan<T> : IRange<T>, IComparable<LessThan<T>>, IEquatable<LessThan<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		public LessThan(T x)
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
				return default;
			}
		}

		public T? UpperLimit
		{
			get
			{
				return Value;
			}
		}

		public int CompareTo(LessThan<T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			return UpperLimit!.CompareTo(other.UpperLimit!);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is LessThan<T>)
			{
				LessThan<T>? o = other as LessThan<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(LessThan<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return UpperLimit!.Equals(other.UpperLimit!);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			LessThan<T>? o = other as LessThan<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			LessThan<T>? o = other as LessThan<T>;
			return Equals(o);
		}

		public static bool operator ==(LessThan<T>? lhs, LessThan<T>? rhs)
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

		public static bool operator !=(LessThan<T>? lhs, LessThan<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			return UpperLimit!.GetHashCode();
		}

		public bool Invoke(T x)
		{
			return 0 < Value.CompareTo(x);
		}

		public bool IsLowerBounded()
		{
			return false;
		}

		public bool IsUpperBounded()
		{
			return true;
		}

		public override string ToString()
		{
			return "(LessThan " + Value + ")";
		}
	}
}
