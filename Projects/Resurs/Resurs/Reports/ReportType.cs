using System.ComponentModel;

namespace Resurs.Reports
{
	public enum ReportType
	{
		[Description("История расхода счетчика")]
		ChangeFlow,

		[Description("Должники")]
		Debtors,
	}
}