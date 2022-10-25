namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.SqlServer
{
	public enum DateNumericMonthFormatEnum
	{
		/// <summary>[m]m/dd/[yy]yy</summary>
		Mdy,
		/// <summary>mm/[yy]yy/dd</summary>
		Myd,
		/// <summary>dd/[m]m/[yy]yy</summary>
		Dmy,
		/// <summary>dd/[yy]yy/[m]m</summary>
		Dym,
		/// <summary>[yy]yy/[m]m/dd</summary>
		Ymd,
	}
}
