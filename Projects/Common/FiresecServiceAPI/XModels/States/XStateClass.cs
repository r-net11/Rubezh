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

		[DescriptionAttribute("Неисправность")]
		Failure = 3,

		[DescriptionAttribute("Отключено")]
		Ignore = 4,

		[DescriptionAttribute("Включено")]
		On = 5,

		[DescriptionAttribute("Включается")]
		TurningOn = 6,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 7,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 8,

		[DescriptionAttribute("Информация")]
		Info = 9,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 10,

		[DescriptionAttribute("Норма")]
		Norm = 11,

		[DescriptionAttribute("Нет")]
		No = 12
	}
}