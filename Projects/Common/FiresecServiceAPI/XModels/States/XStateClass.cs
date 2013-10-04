using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
		[DescriptionAttribute("Отсутствует лицензия")]
		HasNoLicense = 0,

		[DescriptionAttribute("База данных ПК не совпадает с прибором")]
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

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 11,

		[DescriptionAttribute("Выключено")]
		Off = 12,

		[DescriptionAttribute("Выключается")]
		TurningOff = 13,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 14,

		[DescriptionAttribute("Тест")]
		Test = 15,

		[DescriptionAttribute("Информация")]
		Info = 16,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 17,

		[DescriptionAttribute("Норма")]
		Norm = 18,

		[DescriptionAttribute("Нет")]
		No = 19
	}
}