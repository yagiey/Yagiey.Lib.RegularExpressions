using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class LessThanOrEqual<T> : IRange<T>, IComparable<LessThanOrEqual<T>>, IEquatable<LessThanOrEqual<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly Tuple<LessThan<T>, Equal<T>> _or;

		public LessThanOrEqual(T x)
		{
			_or = Tuple.Create(new LessThan<T>(x), new Equal<T>(x));
		}

		public T Value
		{
			get
			{
				return _or.Item1.Value;
			}
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

		public int CompareTo(LessThanOrEqual<T>? other)
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
			else if (other is LessThanOrEqual<T>)
			{
				LessThanOrEqual<T>? o = other as LessThanOrEqual<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(LessThanOrEqual<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return UpperLimit!.Equals(other.UpperLimit!);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			LessThanOrEqual<T>? o = other as LessThanOrEqual<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			LessThanOrEqual<T>? o = other as LessThanOrEqual<T>;
			return Equals(o);
		}

		public static bool operator ==(LessThanOrEqual<T>? lhs, LessThanOrEqual<T>? rhs)
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

		public static bool operator !=(LessThanOrEqual<T>? lhs, LessThanOrEqual<T>? rhs)
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
			return _or.Item1.Invoke(x) || _or.Item2.Invoke(x);
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
			return "(LessThanOrEqual " + _or.Item1.Value + ")";
		}
	}
}
