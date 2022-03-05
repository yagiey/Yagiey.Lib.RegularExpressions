using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
		where T : IEquatable<T>, IComparable<T>
	{
		public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			else if (x == null)
			{
				return false;
			}
			else if (y == null)
			{
				return false;
			}
			if (!x.Any() && !y.Any())
			{
				return true;
			}
			else if (!x.Any())
			{
				return false;
			}
			else if (!y.Any())
			{
				return false;
			}
			else
			{
				bool result = x.First().Equals(y.First());
				if (result)
				{
					return Equals(x.Skip(1), y.Skip(1));
				}
				else
				{
					return false;
				}
			}
		}

		public int GetHashCode([DisallowNull] IEnumerable<T> obj)
		{
			string strObj = string.Join(",", obj.Distinct().OrderBy(_ => _));
			return strObj.GetHashCode();
		}
	}
}
