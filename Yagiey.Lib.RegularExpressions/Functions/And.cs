using System;
using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class And<T> : IUnaryFunctionObject<bool, T>, IComparable<And<T>>, IEquatable<And<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly IEnumerable<IUnaryFunctionObject<bool, T>> _predicates;

		public And(IEnumerable<IUnaryFunctionObject<bool, T>> predicates)
		{
			UnaryFunctionObjectComparer<bool, T> comp = new();
			_predicates = predicates.OrderBy(_ => _, comp);
		}

		public int CompareTo(And<T>? other)
		{
			if (other is null)
			{
				return 1;
			}

			IComparer<IEnumerable<IUnaryFunctionObject<bool, T>>> comp = new EnumerableComparer<IUnaryFunctionObject<bool, T>>();
			return comp.Compare(_predicates, other._predicates);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is And<T>)
			{
				And<T>? o = other as And<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public bool Equals(And<T>? other)
		{
			if (other is null)
			{
				return false;
			}

			IEqualityComparer<IEnumerable<IUnaryFunctionObject<bool, T>>> comp = new EnumerableEqualityComparer<IUnaryFunctionObject<bool, T>>();
			return comp.Equals(_predicates, other._predicates);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			And<T>? o = other as And<T>;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			And<T>? o = other as And<T>;
			return Equals(o);
		}

		public static bool operator ==(And<T>? lhs, And<T>? rhs)
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

		public static bool operator !=(And<T>? lhs, And<T>? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			string strObj = ToString();
			return strObj.GetHashCode();
		}

		public bool Invoke(T x)
		{
			return _predicates.All(pred => pred.Invoke(x));
		}

		public override string ToString()
		{
			return "(And " + string.Join(" ", _predicates.Select(p => p.ToString())) + ")";
		}
	}
}
