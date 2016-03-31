using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Класс состояния
	/// </summary>
	public enum XStateClass
	{
		[DescriptionAttribute("Отсутствует лицензия")]
		HasNoLicense = 0,

		[DescriptionAttribute("База данных прибора не соответствует базе данных ПК")]
		DBMissmatch = 1,

		[DescriptionAttribute("Контроллер в технологическом режиме")]
		TechnologicalRegime = 2,

		[DescriptionAttribute("Потеря связи")]
		ConnectionLost = 3,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 4,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 5,

		[DescriptionAttribute("Внимание")]
		Attention = 6,

		[DescriptionAttribute("Отключено")]
		Ignore = 7,

		[DescriptionAttribute("Неисправность")]
		Failure = 8,

		[DescriptionAttribute("Включено")]
		On = 9,

		[DescriptionAttribute("Включается")]
		TurningOn = 10,

		[DescriptionAttribute("Выключается")]
		TurningOff = 11,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 12,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 13,

		[DescriptionAttribute("Тест")]
		Test = 14,

		[DescriptionAttribute("Информация")]
		Info = 15,

		[DescriptionAttribute("Выключено")]
		Off = 16,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 17,

		[DescriptionAttribute("Норма")]
		Norm = 18,

		[DescriptionAttribute("Нет")]
		No = 19,

		[DescriptionAttribute("Подключение установлено")]
		Connected = 20

	}
}