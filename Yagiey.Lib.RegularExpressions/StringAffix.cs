namespace Yagiey.Lib.RegularExpressions
{
    public class StringAffix
    {
        #region static

        static StringAffix()
        {
            Empty = new(string.Empty, string.Empty, string.Empty);
            SingleQuoted = new(@"'", string.Empty, @"'");
            DoubleQuoted = new(@"""", string.Empty, @"""");
        }

        public static StringAffix Empty;
        public static StringAffix SingleQuoted;
        public static StringAffix DoubleQuoted;

        #endregion

        public string Prefix { get; private set; }
        public string Infix { get; private set; }
        public string Suffix { get; private set; }

        public StringAffix(string prefix, string infix, string suffix)
        {
            Prefix = prefix;
            Infix = infix;
			Suffix = suffix;
        }
    }
}
