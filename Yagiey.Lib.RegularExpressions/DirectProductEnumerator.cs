using System;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions
{
	internal static class DirectProductEnumerator
	{
		public static IEnumerable<Tuple<T1, T2>> DP<T1, T2>(IEnumerable<T1> set1, IEnumerable<T2> set2)
		{
			foreach (var item1 in set1)
			{
				foreach (var item2 in set2)
				{
					yield return Tuple.Create(item1, item2);
				}
			}
		}

		public static IEnumerable<Tuple<T1, T2, T3>> DP<T1, T2, T3>(IEnumerable<T1> set1, IEnumerable<T2> set2, IEnumerable<T3> set3)
		{
			foreach (var item1 in set1)
			{
				foreach (var item2 in set2)
				{
					foreach (var item3 in set3)
					{
						yield return Tuple.Create(item1, item2, item3);
					}
				}
			}
		}

		public static IEnumerable<Tuple<T1, T2, T3, T4>> DP<T1, T2, T3, T4>(IEnumerable<T1> set1, IEnumerable<T2> set2, IEnumerable<T3> set3, IEnumerable<T4> set4)
		{
			foreach (var item1 in set1)
			{
				foreach (var item2 in set2)
				{
					foreach (var item3 in set3)
					{
						foreach (var item4 in set4)
						{
							yield return Tuple.Create(item1, item2, item3, item4);
						}
					}
				}
			}
		}

		public static IEnumerable<Tuple<T1, T2, T3, T4, T5>> DP<T1, T2, T3, T4, T5>(IEnumerable<T1> set1, IEnumerable<T2> set2, IEnumerable<T3> set3, IEnumerable<T4> set4, IEnumerable<T5> set5)
		{
			foreach (var item1 in set1)
			{
				foreach (var item2 in set2)
				{
					foreach (var item3 in set3)
					{
						foreach (var item4 in set4)
						{
							foreach (var item5 in set5)
							{
								yield return Tuple.Create(item1, item2, item3, item4, item5);
							}
						}
					}
				}
			}
		}

		public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> DP<T1, T2, T3, T4, T5, T6>(IEnumerable<T1> set1, IEnumerable<T2> set2, IEnumerable<T3> set3, IEnumerable<T4> set4, IEnumerable<T5> set5, IEnumerable<T6> set6)
		{
			foreach (var item1 in set1)
			{
				foreach (var item2 in set2)
				{
					foreach (var item3 in set3)
					{
						foreach (var item4 in set4)
						{
							foreach (var item5 in set5)
							{
								foreach (var item6 in set6)
								{
									yield return Tuple.Create(item1, item2, item3, item4, item5, item6);
								}
							}
						}
					}
				}
			}
		}
	}
}
