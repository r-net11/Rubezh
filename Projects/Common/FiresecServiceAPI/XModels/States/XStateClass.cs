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

		[DescriptionAttribute("Включается")]
		TurningOn = 5,

		[DescriptionAttribute("Выключается")]
		TurningOff = 6,

		[DescriptionAttribute("Неисправность")]
		Failure = 7,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 8,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 9,

		[DescriptionAttribute("Информация")]
		Info = 10,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 11,

		[DescriptionAttribute("Норма")]
		Norm = 12,

		[DescriptionAttribute("Нет")]
		No = 13
	}
}