namespace Yagiey.Lib.RegularExpressions
{
	internal class Pair<T, U>
	{
		public T Head { get; set; }

		public U Tail { get; set; }

		public Pair(T head, U tail)
		{
			Head = head;
			Tail = tail;
		}
	}
}
