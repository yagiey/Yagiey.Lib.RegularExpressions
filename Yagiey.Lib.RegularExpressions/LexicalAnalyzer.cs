using System;
using System.Collections;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions
{
	internal class LexicalAnalyzer : IEnumerable<Tuple<char, char?[]>>
	{
		public string Source
		{
			get;
			private set;
		}

		public LexicalAnalyzer(string source)
		{
			Source = source;
		}

		#region IEnumerable

		public IEnumerator<Tuple<char, char?[]>> GetEnumerator()
		{
			Func<IEnumerable<char>, int, IEnumerable<Tuple<char, char?[]>>> func = Extensions.Generic.ValueType.EnumerableExtension.EnumerateLookingAhead;
			return func(Source, 2).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
