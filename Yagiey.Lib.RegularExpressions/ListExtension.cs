﻿using System;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions
{
	internal static class ListExtension
	{
		public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate(list[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
