using System;
using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class EnumerableComparer<T> : IComparer<IEnumerable<T>>
		where T : IEquatable<T>, IComparable<T>
	{
		public int Compare(IEnumerable<T>? x, IEnumerable<T>? y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			else if (x == null)
			{
				return -1;
			}
			else if (y == null)
			{
				return 1;
			}
			if (!x.Any() && !y.Any())
			{
				return 0;
			}
			else if (!x.Any())
			{
				return -1;
			}
			else if (!y.Any())
			{
				return 1;
			}
			else
			{
				int result = x.First().CompareTo(y.First());
				if (result == 0)
				{
					return Compare(x.Skip(1), y.Skip(1));
				}
				else
				{
					return result;
				}
			}
		}
	}
}
