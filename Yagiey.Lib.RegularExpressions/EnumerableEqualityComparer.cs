using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		public IComparer<T> Comparer
		{
			get;
			private set;
		}

		public EnumerableEqualityComparer() : this(Comparer<T>.Default)
		{
		}

		public EnumerableEqualityComparer(IComparer<T> comp)
		{
			Comparer = comp;
		}

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
				int result = Comparer.Compare(x.First(), y.First());
				if (result == 0)
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
			string strObj = string.Join(",", obj.Distinct().OrderBy(_ => _, Comparer));
			return strObj.GetHashCode();
		}
	}
}
