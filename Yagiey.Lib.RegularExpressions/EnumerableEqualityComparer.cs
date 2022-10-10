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
			else if (!x.Any() || !y.Any())
			{
				return false;
			}

			var itorX = x.GetEnumerator();
			var itorY = y.GetEnumerator();

			while (true)
			{
				bool hasNextX = itorX.MoveNext();
				bool hasNextY = itorY.MoveNext();

				if (!hasNextX && !hasNextY)
				{
					return true;
				}
				else if (!hasNextX || !hasNextY)
				{
					return false;
				}

				var itemX = itorX.Current;
				var itemY = itorY.Current;

				bool result = itemX.Equals(itemY);
				if (result)
				{
					continue;
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
