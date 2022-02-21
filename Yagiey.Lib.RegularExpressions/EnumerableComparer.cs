using System.Collections.Generic;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{
	internal class EnumerableComparer<T> : IComparer<IEnumerable<T>>
	{
		private readonly IComparer<T> _comp;

		public EnumerableComparer() : this(Comparer<T>.Default)
		{
		}

		public EnumerableComparer(IComparer<T> comp)
		{
			_comp = comp;
		}

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
				int result = _comp.Compare(x.First(), y.First());
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
