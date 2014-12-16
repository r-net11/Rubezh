using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum CardSortType
	{
		[Description("Тип")]
		Status,

		[Description("Номер")]
		Number,

		[Description("Организация")]
		Organisation,

		[Description("Подразделение")]
		Department,

		[Description("Должность")]
		Position,

		[Description("Сотрудник")]
		Employee,

		[Description("Срок действия")]
		Duration
	}
}
