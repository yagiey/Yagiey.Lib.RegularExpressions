using System;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal class Input : IInput, IComparable<Input>, IEquatable<Input>, IComparable<IInput>, IEquatable<IInput>
	{
		public static Input Empty = new();

		private readonly InputType _inputType;

		public char Character
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get
			{
				return _inputType == InputType.Empty;
			}
		}

		public bool IsLiteral
		{
			get
			{
				return _inputType == InputType.Literal;
			}
		}

		public bool IsPredicate
		{
			get
			{
				return false;
			}
		}

		public Input() : this(InputType.Empty, default)
		{
		}

		public Input(char ch) : this(InputType.Literal, ch)
		{
		}

		private Input(InputType inputType, char ch)
		{
			Character = ch;
			_inputType = inputType;
		}

		public bool Match(char ch)
		{
			if (IsEmpty)
			{
				return false;
			}
			else
			{
				return ch == Character;
			}
		}

		public int CompareTo(Input? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (_inputType != other._inputType)
			{
				return _inputType.CompareTo(other._inputType);
			}
			else if (_inputType == InputType.Empty)
			{
				return 0;
			}
			else
			{
				return Character.CompareTo(other.Character);
			}
		}

		public int CompareTo(IInput? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is Input)
			{
				Input? o = other as Input;
				return CompareTo(o);
			}
			else if (other is InputWithPredicate)
			{
				return -1;
			}
			else
			{
				throw new Exception();
			}
		}

		public bool Equals(Input? other)
		{
			if (other is null)
			{
				return false;
			}
			else if (_inputType != other._inputType)
			{
				return false;
			}
			else if (_inputType == InputType.Empty)
			{
				return true;
			}
			else
			{
				return Character.Equals(other.Character);
			}
		}

		public bool Equals(IInput? other)
		{
			Input? o = other as Input;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			Input? o = other as Input;
			return Equals(o);
		}

		public static bool operator ==(Input? lhs, Input? rhs)
		{
			if (lhs is null && rhs is null)
			{
				return true;
			}
			else if (lhs is null)
			{
				return false;
			}
			else if (rhs is null)
			{
				return false;
			}
			else
			{
				return lhs.Equals(rhs);
			}
		}

		public static bool operator !=(Input lhs, Input rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			int type = (int)_inputType;
			return (type << 16) | Character;
		}

		public override string ToString()
		{
			if (_inputType == InputType.Empty)
			{
				return "<E>";
			}
			else
			{
				return Character.ToString();
			}
		}

		public string ToRegularExpression()
		{
			if (_inputType == InputType.Empty)
			{
				return @"\e";
			}
			else if (Character == Constants.Backslash
				|| Character == Constants.Dot
				|| Character == Constants.LParen
				|| Character == Constants.RParen
				|| Character == Constants.VerticalBar
				|| Character == Constants.Asterisk
				|| Character == Constants.Question
				|| Character == Constants.Hat
				|| Character == Constants.LBracket
				|| Character == Constants.RBracket
				|| Character == Constants.LBrace
				|| Character == Constants.RBrace
				|| Character == Constants.Plus
				|| Character == Constants.Minus
				)
			{
				return string.Format("{0}{1}", Constants.Backslash, Character);
			}
			else if (Character == Constants.Tab)
			{
				return string.Format("{0}{1}", Constants.Backslash, Constants.EscapedTab);
			}
			else if (Character == Constants.CarriageReturn)
			{
				return string.Format("{0}{1}", Constants.Backslash, Constants.EscapedCr);
			}
			else if (Character == Constants.LineFeed)
			{
				return string.Format("{0}{1}", Constants.Backslash, Constants.EscapedLf);
			}
			else
			{
				return Character.ToString();
			}
		}
	}
}
