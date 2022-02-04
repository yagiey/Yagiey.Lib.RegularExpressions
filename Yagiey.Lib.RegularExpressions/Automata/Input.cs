using System;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal struct Input : IComparable<Input>
	{
		public static Input Empty = new();

		public bool IsEmpty
		{
			get;
			private set;
		}

		public char Character
		{
			get;
			private set;
		}

		public Input() : this(default)
		{
			IsEmpty = true;
		}

		public Input(char ch)
		{
			Character = ch;
			IsEmpty = false;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (obj is Input o)
			{
				return this == o;
			}

			return false;
		}

		public static bool operator ==(Input lhs, Input rhs)
		{
			if (lhs.IsEmpty && rhs.IsEmpty)
			{
				return true;
			}
			else if (!lhs.IsEmpty && !rhs.IsEmpty && lhs.Character == rhs.Character)
			{
				return true;
			}
			return false;
		}
		public static bool operator !=(Input lhs, Input rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			return IsEmpty.GetHashCode() ^ Character.GetHashCode();
		}

		public override string ToString()
		{
			if (IsEmpty)
			{
				return "<E>";
			}
			else
			{
				return "'" + Character + "'";
			}
		}

		public int CompareTo(Input other)
		{
			// if (this < other) return -1
			// else if (other < this) return 1
			// else return 0

			if (IsEmpty && other.IsEmpty)
			{
				return 0;
			}
			else if (IsEmpty && !other.IsEmpty)
			{
				return -1;
			}
			else if (!IsEmpty && other.IsEmpty)
			{
				return 1;
			}
			else
			{
				return Character.CompareTo(other.Character);
			}
		}
	}
}
