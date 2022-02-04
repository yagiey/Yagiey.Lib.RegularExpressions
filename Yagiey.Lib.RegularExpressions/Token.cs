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
			return string.Format("[{0}]{1}", Source, TokenType);
		}
	}
}
