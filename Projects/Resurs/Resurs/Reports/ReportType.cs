using System.ComponentModel;

namespace Resurs.Reports
{
	public enum ReportType
	{
		[Description("История расхода счетчика")]
		ChangeFlow,

		[Description("Должники")]
		Debtors,

		[Description("Квитанции абонентов")]
		Receipts,

		[Description("Изменение расхода")]
		ChangeValue
	}
}