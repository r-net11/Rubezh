using System.ComponentModel;

namespace StrazhAPI.GK
{
	/// <summary>
	/// Класс состояния
	/// </summary>
	public enum XStateClass
	{
		[Description("Лицензия отсутствует")]
		HasNoLicense = 0,

		[Description("База данных устройства не соответствует базе данных ПК")]
		DBMissmatch = 1,

		[Description("Контроллер в технологическом режиме")]
		TechnologicalRegime = 2,

		[Description("Потеря связи")]
		ConnectionLost = 3,

		[Description("Пожар 2")]
		Fire2 = 4,

		[Description("Пожар 1")]
		Fire1 = 5,

		[Description("Тревога")]
		Attention = 6,

		[Description("Отключено")]
		Ignore = 7,

		[Description("Неисправность")]
		Failure = 8,

		[Description("Включено")]
		On = 9,

		[Description("Включается")]
		TurningOn = 10,

		[Description("Выключается")]
		TurningOff = 11,

		[Description("Автоматика отключена")]
		AutoOff = 12,

		[Description("Требуется обслуживание")]
		Service = 13,

		[Description("Тест")]
		Test = 14,

		[Description("Информация")]
		Info = 15,

		[Description("Выключено")]
		Off = 16,

		[Description("Неизвестно")]
		Unknown = 17,

		[Description("Норма")]
		Norm = 18,

		[Description("Нет")]
		No = 19,

		[Description("Лицензия обнаружена")]
		HasLicense = 20,
	}
}