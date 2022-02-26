namespace Yagiey.Lib.RegularExpressions
{
	internal class Token
	{
		public string Source
		{
			get;
			private set;
		}

		public TokenType TokenType
		{
			get;
			private set;
		}

		public Token(string src, TokenType type)
		{
			Source = src;
			TokenType = type;
		}

		public override string ToString()
		{
			string src = TokenType == TokenType.Escape ? Constants.Backslash + Source : Source;
			//return string.Format("[{0}]{1}", src, TokenType);
			return src;
		}

		public bool CanBeRegardedAsRestOfConcatenation()
		{
			char ch = Source[0];
			return
				(TokenType == TokenType.Character
				&& ch != Constants.VerticalBar
				&& ch != Constants.Asterisk
				&& ch != Constants.Question
				&& ch != Constants.Hat
				&& ch != Constants.RParen
				&& ch != Constants.RBracket
				&& ch != Constants.RBrace)
				|| TokenType == TokenType.Escape
				;
		}

		public bool CanBeRegardedAsRestOfSelection()
		{
			return
				TokenType == TokenType.Character
				&& Source[0] == Constants.VerticalBar
				;
		}

		public bool IsBeginingOfExpressionWithParen()
		{
			return
				TokenType == TokenType.Character
				&& Source[0] == Constants.LParen
				;
		}

		public bool IsEndOfExpressionWithParen()
		{
			return
				TokenType == TokenType.Character
				&& Source[0] == Constants.RParen
				;
		}
	}
}
