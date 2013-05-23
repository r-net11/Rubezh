using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
		[DescriptionAttribute("Пожар 2")]
		Fire2 = 0,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 1,

		[DescriptionAttribute("Внимание")]
		Attention = 2,

		[DescriptionAttribute("Отключено")]
		Ignore = 3,

		[DescriptionAttribute("Включено")]
		On = 4,

		[DescriptionAttribute("Выключено")]
		Off = 5,

		[DescriptionAttribute("Включается")]
		TurningOn = 6,

		[DescriptionAttribute("Выключается")]
		TurningOff = 7,

		[DescriptionAttribute("Неисправность")]
		Failure = 8,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 9,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 10,

		[DescriptionAttribute("Информация")]
		Info = 11,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 12,

		[DescriptionAttribute("Норма")]
		Norm = 13,

		[DescriptionAttribute("Нет")]
		No = 14
	}
}