using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yagiey.Lib.RegularExpressions
{

	internal class IntSetEqualityComparer : IEqualityComparer<IEnumerable<int>>
	{
		public bool Equals(IEnumerable<int>? x, IEnumerable<int>? y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			else if (x == null || y == null)
			{
				return false;
			}
			return x.All(_ => y.Contains(_)) && y.All(_ => x.Contains(_));
		}

		public int GetHashCode([DisallowNull] IEnumerable<int> obj)
		{
			string strObj = string.Join(",", obj.Distinct().OrderBy(_ => _));
			return strObj.GetHashCode();
		}
	}
}
