using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum XBaseObjectType
	{
		[DescriptionAttribute("Устройство")]
		Deivce,

		[DescriptionAttribute("Зона")]
		Zone,

		[DescriptionAttribute("Направление")]
		Direction,

		[DescriptionAttribute("НС")]
		PumpStation,

		[DescriptionAttribute("МПТ")]
		MPT,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("ПИМ")]
		Pim,

		[DescriptionAttribute("Охранная Зона")]
		GuardZone,

		[DescriptionAttribute("Код")]
		Code,

		[DescriptionAttribute("Точка доступа")]
		Door,
	}
}