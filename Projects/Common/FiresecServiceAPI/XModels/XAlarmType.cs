using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XAlarmType
	{
		[DescriptionAttribute("Пожаротушение")]
		NPTOn = 0,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 1,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 2,

		[DescriptionAttribute("Внимание")]
		Attention = 3,

		[DescriptionAttribute("Неисправность")]
		Failure = 4,

		[DescriptionAttribute("Отключенное оборудование")]
		Ignore = 5,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 6,

		[DescriptionAttribute("Требуется обслуживание(запыленность)")]
		Service = 7,

		[DescriptionAttribute("Включается/Включено")]
		Turning = 8
	}
}