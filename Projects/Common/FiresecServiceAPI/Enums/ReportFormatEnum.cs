using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum ReportFormatEnum
	{
		[Description("*.pdf")]
		Pdf,
		[Description("*.html")]
		Html,
		[Description("*.mht")]
		Mht,
		[Description("*.rtf")]
		Rtf,
		[Description("*.xls")]
		Xls,
		[Description("*.xlsx")]
		Xlsx,
		[Description("*.csv")]
		Csv,
		[Description("*.txt")]
		Txt,
		[Description("*.png")]
		Png,
		//[Description("*.xps")]
		//Xps
	}
}
