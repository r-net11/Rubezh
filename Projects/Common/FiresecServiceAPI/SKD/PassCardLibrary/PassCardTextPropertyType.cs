using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum PassCardTextPropertyType
	{
		//[Description("Фамилия")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "LastName")]
		LastName,

        //[Description("Имя")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "FirstName")]
		FirstName,

        //[Description("Отчество")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "SecondName")]
		SecondName,

        //[Description("Дата рождения")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "Birthday")]
		Birthday,

        //[Description("Организация")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "Organisation")]
		Organisation,

        //[Description("Подразделение")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "Department")]
		Department,

        //[Description("Должность")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "Position")]
		Position,

        //[Description("Начало срока дейстия")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "StartDate")]
		StartDate,

        //[Description("Конец срока дейстия")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "EndDate")]
		EndDate,

        //[Description("Номер карты")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "CardNumber")]
		CardNumber,

        //[Description("Дополнительно")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardTextPropertyType), "Additional")]
		Additional,
	}
}