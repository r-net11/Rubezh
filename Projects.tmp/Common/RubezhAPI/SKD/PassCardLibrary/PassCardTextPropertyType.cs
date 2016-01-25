using System.ComponentModel;

namespace RubezhAPI.SKD
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
		[Description("Конец срока дейстия")]
		EndDate,
		[Description("Номер карты")]
		CardNumber,
		[Description("Дополнительно")]
		Additional,
	}
}