using System;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal class InputWithPredicate : IInput, IComparable<InputWithPredicate>, IEquatable<InputWithPredicate>, IComparable<IInput>, IEquatable<IInput>
	{
		private readonly IUnaryFunctionObject<bool, char> _predicate;

		public bool IsEmpty
		{
			get
			{
				return false;
			}
		}

		public bool IsLiteral
		{
			get
			{
				return false;
			}
		}

		public bool IsPredicate
		{
			get
			{
				return true;
			}
		}

		public InputWithPredicate(IUnaryFunctionObject<bool, char> predicate)
		{
			_predicate = predicate;
		}

		public bool Match(char ch)
		{
			return _predicate.Invoke(ch);
		}

		public int CompareTo(InputWithPredicate? other)
		{
			if (other is null)
			{
				return 1;
			}
			return _predicate.CompareTo(other._predicate);
		}

		public int CompareTo(IInput? other)
		{
			if (other is null)
			{
				return 1;
			}
			else if (other is InputWithPredicate)
			{
				InputWithPredicate? o = other as InputWithPredicate;
				return CompareTo(o);
			}
			else if (other is Input)
			{
				return 1;
			}
			else
			{
				throw new Exception();
			}
		}

		public bool Equals(InputWithPredicate? other)
		{
			if (other is null)
			{
				return false;
			}
			return _predicate.Equals(other._predicate);
		}

		public bool Equals(IInput? other)
		{
			InputWithPredicate? o = other as InputWithPredicate;
			return Equals(o);
		}

		public override bool Equals(object? other)
		{
			InputWithPredicate? o = other as InputWithPredicate;
			return Equals(o);
		}

		public static bool operator ==(InputWithPredicate? lhs, InputWithPredicate? rhs)
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

		public static bool operator !=(InputWithPredicate? lhs, InputWithPredicate? rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			return _predicate.GetHashCode();
		}

		public override string ToString()
		{
			return _predicate.ToString()!;
		}

		public string ToRegularExpression()
		{
			return _predicate.ToRegularExpression();
		}
	}
}
