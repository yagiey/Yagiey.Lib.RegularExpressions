﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yagiey.Lib.RegularExpressions.Functions
{
	internal class Or<T> : IUnaryFunctionObject<bool, T>, IComparable<Or<T>>, IEquatable<Or<T>>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly IEnumerable<IUnaryFunctionObject<bool, T>> _predicates;

		public Or(IEnumerable<IUnaryFunctionObject<bool, T>> predicates)
		{
			UnaryFunctionObjectComparer<bool, T> comp = new();
			_predicates = predicates.OrderBy(_ => _, comp);
		}

		public int CompareTo(IUnaryFunctionObject<bool, T>? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is Or<T>)
			{
				Or<T>? o = other as Or<T>;
				return CompareTo(o);
			}
			else
			{
				UnaryFunctionObjectComparer<bool, T> comp = new();
				return comp.Compare(this, other);
			}
		}

		public int CompareTo(Or<T>? other)
		{
			if (other is null)
			{
				return 1;
			}

			IComparer<IEnumerable<IUnaryFunctionObject<bool, T>>> comp = new EnumerableComparer<IUnaryFunctionObject<bool, T>>();
			return comp.Compare(_predicates, other._predicates);
		}

		public bool Equals(IUnaryFunctionObject<bool, T>? other)
		{
			Or<T>? o = other as Or<T>;
			return Equals(o);
		}

		public bool Equals(Or<T>? other)
		{
			if (other is null)
			{
				return false;
			}

			IEqualityComparer<IEnumerable<IUnaryFunctionObject<bool, T>>> comp = new EnumerableEqualityComparer<IUnaryFunctionObject<bool, T>>();
			return comp.Equals(_predicates, other._predicates);
		}

		public override bool Equals(object? other)
		{
			if (other is not Or<T> o)
			{
				return false;
			}
			return Equals(o);
		}

		public static bool operator ==(Or<T>? lhs, Or<T>? rhs)
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

		public static bool operator !=(Or<T>? lhs, Or<T>? rhs)
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
			return _predicates.Any(pred => pred.Invoke(x));
		}

		public override string ToString()
		{
			return "(Or " + string.Join(" ", _predicates.Select(p => p.ToString())) + ")";
		}

		public string ToRegularExpression()
		{
			StringBuilder sb = new();
			foreach (var item in _predicates)
			{
				if (item.GetType() != typeof(Equal<>) && item.GetType() != typeof(GELE<>))
				{
					throw new Exception();
				}
				sb.Append(item.ToRegularExpression());
			}
			return string.Format("[{0}]", sb.ToString());
		}
	}
}
