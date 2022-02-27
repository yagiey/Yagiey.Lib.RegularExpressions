using System;

namespace Yagiey.Lib.RegularExpressions
{
	internal static class Token
	{
		public static bool CanNextBeRegardedAsRestOfConcatenation(this Tuple<char, char?[]> currentItem)
		{
			char? next = currentItem.Item2[0];
			if (next == null)
			{
				return false;
			}

			return
				(
					next != Constants.VerticalBar
					&& next.Value != Constants.Question
					&& next.Value != Constants.Asterisk
					&& next.Value != Constants.Hat
					&& next.Value != Constants.RParen
					&& next.Value != Constants.RBracket
					&& next.Value != Constants.RBrace
				)
				|| next.Value == Constants.Backslash
				;
		}

		public static bool CanNextBeRegardedAsRestOfSelection(this Tuple<char, char?[]> currentItem)
		{
			char? next = currentItem.Item2[0];
			if (next == null)
			{
				return false;
			}
			return next == Constants.VerticalBar;
		}

		public static bool IsBeginingOfExpressionWithParen(this Tuple<char, char?[]> currentItem)
		{
			return currentItem.Item1 == Constants.LParen;
		}

		public static bool IsNextEndOfExpressionWithParen(this Tuple<char, char?[]> currentItem)
		{
			char? next = currentItem.Item2[0];
			if (next == null)
			{
				return false;
			}
			return next == Constants.RParen;
		}

		public static bool IsBeginingOfCharacterClassWithBracket(this Tuple<char, char?[]> currentItem)
		{
			return currentItem.Item1 == Constants.LBracket;
		}

		public static bool IsNextEndOfCharacterClassWithBracket(this Tuple<char, char?[]> currentItem)
		{
			char? next = currentItem.Item2[0];
			if (next == null)
			{
				return false;
			}
			return next == Constants.RBracket;
		}
	}
}
