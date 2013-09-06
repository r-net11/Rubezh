using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
		[DescriptionAttribute("База данных ПК не совпадает с прибором")]
		DBMissmatch = 0,

		[DescriptionAttribute("Контроллер в технологическом режиме")]
		TechnologicalRegime = 1,

		[DescriptionAttribute("Потеря связи")]
		ConnectionLost = 2,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 3,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 4,

		[DescriptionAttribute("Внимание")]
		Attention = 5,

		[DescriptionAttribute("Отключено")]
		Ignore = 6,

        [DescriptionAttribute("Неисправность")]
        Failure = 7,

		[DescriptionAttribute("Включено")]
		On = 8,

		[DescriptionAttribute("Выключено")]
		Off = 9,

		[DescriptionAttribute("Включается")]
		TurningOn = 10,

		[DescriptionAttribute("Выключается")]
		TurningOff = 11,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 12,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 13,

		[DescriptionAttribute("Информация")]
		Info = 14,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 15,

		[DescriptionAttribute("Норма")]
		Norm = 16,

		[DescriptionAttribute("Нет")]
		No = 17
	}
}