using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum XBaseObjectType
	{
		[DescriptionAttribute("Устройство")]
		Deivce,

		[DescriptionAttribute("Зона")]
		Zone,

		[DescriptionAttribute("Охранная Зона")]
		GuardZone,

		[DescriptionAttribute("Направление")]
		Direction,

		[DescriptionAttribute("НС")]
		PumpStation,

		[DescriptionAttribute("МПТ")]
		MPT,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("ПИМ")]
		Pim
	}
}