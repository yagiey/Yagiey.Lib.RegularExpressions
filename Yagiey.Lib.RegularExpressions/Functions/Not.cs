using System;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class Not<T> : IUnaryFunctionObject<bool, T>, IComparable<Not<T>>, IEquatable<Not<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly IUnaryFunctionObject<bool, T> _predicate;

		public Not(IUnaryFunctionObject<bool, T> predicate)
		{
			_predicate = predicate;
		}

		public int CompareTo(Not<T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			return _predicate.CompareTo(other._predicate);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is Not<T>)
			{
				Not<T>? o = other as Not<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(Not<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			return _predicate.Equals(other._predicate);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			Not<T>? o = other as Not<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			Not<T>? o = other as Not<T>;
			return Equals(o);
		}

		public static bool operator ==(Not<T>? lhs, Not<T>? rhs)
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

		public static bool operator !=(Not<T>? lhs, Not<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			string strObj = ToString();
			return strObj.GetHashCode();
		}

		public bool Invoke(T arg)
		{
			return !_predicate.Invoke(arg);
		}

		public override string ToString()
		{
			return "(NOT " + _predicate + ")";
		}

		public string ToRegularExpression()
		{
			return string.Format("^{0}", _predicate.ToRegularExpression());
		}
	}
}
