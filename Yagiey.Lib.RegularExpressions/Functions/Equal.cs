using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class Equal<T> : IUnaryFunctionObject<bool, T>, IComparable<Equal<T>>, IEquatable<Equal<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		public Equal(T x)
		{
			Value = x;
		}

		public T Value
		{
			get;
			private set;
		}

		public int CompareTo(Equal<T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			return Value.CompareTo(other.Value);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is Equal<T>)
			{
				Equal<T>? o = other as Equal<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(Equal<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return Value.Equals(other.Value);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			Equal<T>? o = other as Equal<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			Equal<T>? o = other as Equal<T>;
			return Equals(o);
		}

		public static bool operator ==(Equal<T>? lhs, Equal<T>? rhs)
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

		public static bool operator !=(Equal<T>? lhs, Equal<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public bool Invoke(T x)
		{
			return 0 == Value.CompareTo(x);
		}

		public override string ToString()
		{
			return String.Format("(Equal (ch {0}))", Convert.ToInt32(Value));
		}
	}
}
