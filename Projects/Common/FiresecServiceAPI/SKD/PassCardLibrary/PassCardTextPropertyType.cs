using System.ComponentModel;

namespace FiresecAPI.SKD.PassCardLibrary
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
		[Description("Дополнительно")]
		Additional,
	}
}