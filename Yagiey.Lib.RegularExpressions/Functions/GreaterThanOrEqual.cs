using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class GreaterThanOrEqual<T> : IRange<T>, IComparable<GreaterThanOrEqual<T>>, IEquatable<GreaterThanOrEqual<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly Tuple<GreaterThan<T>, Equal<T>> _or;

		public GreaterThanOrEqual(T x)
		{
			_or = Tuple.Create(new GreaterThan<T>(x), new Equal<T>(x));
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
			get { return Value; }
		}

		public T? UpperLimit
		{
			get { return default; }
		}

		public int CompareTo(GreaterThanOrEqual<T>? other)
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
			else if (other is GreaterThanOrEqual<T>)
			{
				GreaterThanOrEqual<T>? o = other as GreaterThanOrEqual<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(GreaterThanOrEqual<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return LowerLimit!.Equals(other.LowerLimit!);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			GreaterThanOrEqual<T>? o = other as GreaterThanOrEqual<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			GreaterThanOrEqual<T>? o = other as GreaterThanOrEqual<T>;
			return Equals(o);
		}

		public static bool operator ==(GreaterThanOrEqual<T>? lhs, GreaterThanOrEqual<T>? rhs)
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

		public static bool operator !=(GreaterThanOrEqual<T>? lhs, GreaterThanOrEqual<T>? rhs)
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
			return _or.Item1.Invoke(x) || _or.Item2.Invoke(x);
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
			return "(GreaterThanOrEqual " + _or.Item1.Value + ")";
		}
	}
}
