using System.ComponentModel;

namespace XFiresecAPI
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

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("ПИМ")]
		Pim
	}
}