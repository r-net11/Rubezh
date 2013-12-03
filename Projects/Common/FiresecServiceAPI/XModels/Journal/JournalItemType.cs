using System.ComponentModel;

namespace XFiresecAPI
{
	public enum JournalItemType
	{
		[DescriptionAttribute("Система")]
		System,

		[DescriptionAttribute("ГК")]
		GK,

		[DescriptionAttribute("Устройство")]
		Device,

		[DescriptionAttribute("Зона")]
		Zone,

		[DescriptionAttribute("Направление")]
		Direction,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("НС")]
		PumpStation,

		[DescriptionAttribute("ПИМ")]
		Pim,

		[DescriptionAttribute("Пользователь прибора")]
		GkUser
	}
}