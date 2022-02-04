using System;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions.Extensions.Generic
{
	internal static class EnumerableExtension
	{
		public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
		{
			foreach (T item in e)
			{
				action(item);
			}
		}
	}
}
