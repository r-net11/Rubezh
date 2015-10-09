using System.ComponentModel;

namespace Resurs.Reports
{
	public enum ReportType
	{
		[Description("Изменение расхода счетчика")]
		ChangeFlow,

		[Description("Данные по должникам")]
		Debtors,

		[Description("Отчет по квитанциям")]
		Receipts,

		[Description("Изменение расхода счетчиков")]
		ChangeValue
	}
}