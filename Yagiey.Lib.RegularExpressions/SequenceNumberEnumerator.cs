using System;
using System.Collections;
using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions
{
	internal class SequenceNumberEnumerator : IEnumerator<int>
	{
		private readonly int _start = 0;

		public SequenceNumberEnumerator(int start)
		{
			if (start <= int.MinValue)
			{
				throw new ArgumentException("invalid argument", nameof(start));
			}
			_start = start;
			Reset();
		}

		public int Current
		{
			get;
			private set;
		}

		object IEnumerator.Current => throw new NotImplementedException();

		public void Dispose()
		{
			// nothing to do
		}

		public bool MoveNext()
		{
			Current++;
			return Current == int.MaxValue;
		}

		public void Reset()
		{
			Current = _start - 1;
		}
	}
}
