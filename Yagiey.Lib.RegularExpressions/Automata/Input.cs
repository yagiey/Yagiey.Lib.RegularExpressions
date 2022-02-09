using System;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal struct Input : IComparable<Input>, IEquatable<Input>
	{
		public static Input Empty = new();

		public InputType InputType
		{
			get;
			private set;
		}

		public char Character
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get
			{
				return InputType == InputType.Empty;
			}
		}

		public bool IsAny
		{
			get
			{
				return InputType == InputType.Any;
			}
		}

		public bool IsMatch
		{
			get
			{
				return InputType == InputType.Match;
			}
		}

		public Input() : this(default)
		{
			InputType = InputType.Empty;
		}

		public Input(char ch)
		{
			Character = ch;
			InputType = InputType.Match;
		}

		public bool Equals(Input other)
		{
			return this == other;
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
			if (lhs.InputType != rhs.InputType)
			{
				return false;
			}
			else if (lhs.InputType == InputType.Empty)
			{
				return true;
			}
			else if (lhs.InputType == InputType.Any)
			{
				return true;
			}
			else if (lhs.InputType == InputType.Match && lhs.Character == rhs.Character)
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
			int type = (int)InputType;
			int ch = Character;
			return (type << 16) | ch;
		}

		public override string ToString()
		{
			if (InputType == InputType.Empty)
			{
				return "<E>";
			}
			else if (InputType == InputType.Any)
			{
				return "'.'";
			}
			else
			{
				return "'" + Character + "'";
			}
		}

		public int CompareTo(Input other)
		{
			if (InputType != other.InputType)
			{
				return InputType.CompareTo(other.InputType);
			}
			else if (InputType == InputType.Empty)
			{
				return 0;
			}
			else if (InputType == InputType.Any)
			{
				return 0;
			}
			else
			{
				return Character.CompareTo(other.Character);
			}
		}
	}
}
