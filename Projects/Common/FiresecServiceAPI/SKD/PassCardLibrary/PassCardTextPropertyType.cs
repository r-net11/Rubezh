using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum PassCardTextPropertyType
	{
		[Description("Фамилия")]
		LastName,

		[Description("Имя")]
		FirstName,

		[Description("Отчество")]
		SecondName,

		[Description("Дата рождения")]
		Birthday,

		[Description("Организация")]
		Organisation,

		[Description("Подразделение")]
		Department,

		[Description("Должность")]
		Position,

		[Description("Начало срока дейстия")]
		StartDate,

		[Description("Конец срока дейстия")]
		EndDate,

		[Description("Номер карты")]
		CardNumber,

		[Description("Дополнительно")]
		Additional,
	}
}