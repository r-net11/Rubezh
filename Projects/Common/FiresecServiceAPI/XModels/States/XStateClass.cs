using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
		[DescriptionAttribute("База данных ПК не совпадает с прибором")]
		DBMissmatch = 0,

		[DescriptionAttribute("Потеря связи")]
		ConnectionLost = 1,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 2,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 3,

		[DescriptionAttribute("Внимание")]
		Attention = 4,

		[DescriptionAttribute("Отключено")]
		Ignore = 5,

        [DescriptionAttribute("Неисправность")]
        Failure = 6,

		[DescriptionAttribute("Включено")]
		On = 7,

		[DescriptionAttribute("Выключено")]
		Off = 8,

		[DescriptionAttribute("Включается")]
		TurningOn = 9,

		[DescriptionAttribute("Выключается")]
		TurningOff = 10,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 11,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 12,

		[DescriptionAttribute("Информация")]
		Info = 13,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 14,

		[DescriptionAttribute("Норма")]
		Norm = 15,

		[DescriptionAttribute("Нет")]
		No = 16
	}
}